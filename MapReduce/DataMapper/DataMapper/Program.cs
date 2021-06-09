using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Transfer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DataMapper {

    public sealed class Program {

        private readonly IAmazonDynamoDB _dynamoService;
        private readonly IAmazonS3 _s3Service;
        private readonly SqlConnection _dbConnection;

        private const string MapReduceDataBucket = "cosc2640a3.map.reduce.data";
        private const string MapReduceDataTable = "cosc2640a3.map.reduce.data";
        private const string MapReduceProgressTable = "cosc2640a3.map.reduce.progress";
        private const string DbConnectionString = "Server=cca3db.ctfrqvuved4d.ap-southeast-2.rds.amazonaws.com;" +
            "Database=COSC2640A3;User ID=admin;Password=cca3dbpassword;Trusted_Connection=True;Integrated Security=False;";
            //Server=(localdb)\MSSQLLocalDB;Database=COSC2640A3;Trusted_Connection=True;

        public Program() {
            _dynamoService = new AmazonDynamoDBClient(
                "AKIAJSENDXCAPZWGB6HQ",
                "HeGULGolRgnxwKIIm4K2d8E+sAoHVBukvR+5umU3",
                new AmazonDynamoDBConfig {
                    RegionEndpoint = RegionEndpoint.APSoutheast2
                }
            );

            _s3Service = new AmazonS3Client(
                "AKIAJSENDXCAPZWGB6HQ",
                "HeGULGolRgnxwKIIm4K2d8E+sAoHVBukvR+5umU3",
                new AmazonS3Config {
                    RegionEndpoint = RegionEndpoint.APSoutheast2,
                    Timeout = TimeSpan.FromSeconds(120)
                }
            );

            _dbConnection = new SqlConnection(DbConnectionString);
        }

        public static async Task Main(string[] args) {
            var program = new Program();
            var progressId = await SaveProgress(program._dynamoService);

            var query = "SELECT E.Id, I.DueAmount FROM Enrolment E, Invoice I WHERE E.InvoiceId = I.Id;";
            var command = new SqlCommand(query, program._dbConnection);

            program._dbConnection.Open();
            using var reader = command.ExecuteReader();

            var enrolmentInvoices = new List<EnrolmentInvoice>();
            while (reader.Read()) {
                var item = new EnrolmentInvoice {
                    Id = reader[nameof(EnrolmentInvoice.Id)].ToString(),
                    DueAmount = double.Parse(reader[nameof(EnrolmentInvoice.DueAmount)].ToString())
                };

                enrolmentInvoices.Add(item);
            }

            program._dbConnection.Close();
            var fileId = await SaveDataToS3Bucket(JsonConvert.SerializeObject(enrolmentInvoices.ToArray()), program._s3Service);
            if (fileId == null) Environment.Exit(-1);

            await SaveToDynamoAsync(fileId, program._dynamoService);
            await SaveProgress(program._dynamoService, progressId, false);
            Environment.Exit(1);
        }

        private static async Task<string> SaveProgress(IAmazonDynamoDB dynamoDB, string key = null, bool processing = true) {
            Table progressTable = Table.LoadTable(dynamoDB, MapReduceProgressTable);

            if (string.IsNullOrEmpty(key)) {
                key = Guid.NewGuid().ToString().ToLower();
                var progress = new Document {
                    ["Id"] = key,
                    ["Timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    ["MapperDone"] = !processing,
                    ["ReducerDone"] = false
                };

                _ = await progressTable.PutItemAsync(progress);
                return key;
            }

            var updateItemRequest = new UpdateItemRequest {
                TableName = MapReduceProgressTable,
                Key = new Dictionary<string, AttributeValue> {
                    { "Id", new AttributeValue { S = key } }
                },
                AttributeUpdates = new Dictionary<string, AttributeValueUpdate> {
                    ["MapperDone"] = new AttributeValueUpdate {
                        Action = AttributeAction.PUT,
                        Value = new AttributeValue { BOOL = !processing }
                    },
                    ["Timestamp"] = new AttributeValueUpdate {
                        Action = AttributeAction.PUT,
                        Value = new AttributeValue { N = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() }
                    }
                },
                ReturnValues = ReturnValue.NONE
            };

            var updateResult = await dynamoDB.UpdateItemAsync(updateItemRequest);
            return updateResult.HttpStatusCode != HttpStatusCode.OK ? null : key;
        }

        private static async Task SaveToDynamoAsync(string fileId, IAmazonDynamoDB dynamoDB) {
            Table mrTable = Table.LoadTable(dynamoDB, MapReduceDataTable);
            await mrTable.PutItemAsync(new Document {
                ["Id"] = Guid.NewGuid().ToString().ToLower(),
                ["FileId"] = fileId,
                ["Timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        private static async Task<string> SaveDataToS3Bucket(string serializedData, IAmazonS3 s3Service) {
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);

            await writer.WriteAsync(serializedData);
            await writer.FlushAsync();

            memoryStream.Position = 0;
            var uploader = new TransferUtility(s3Service);

            try {
                var fileKey = Guid.NewGuid().ToString().ToLower();

                await uploader.UploadAsync(memoryStream, MapReduceDataBucket, fileKey);
                await s3Service.MakeObjectPublicAsync(MapReduceDataBucket, fileKey, true);

                writer.Close();
                return fileKey;
            }
            catch (Exception) {
                await uploader.AbortMultipartUploadsAsync(MapReduceDataBucket, DateTime.UtcNow);
                writer.Close();
                return default;
            }
        }

        public sealed class EnrolmentInvoice {
            [DynamoDBHashKey]
            public string Id { get; set; }
            public double DueAmount { get; set; }

            public Document CreateDocument() {
                return new Document {
                    [nameof(Id)] = Id.ToLower(),
                    [nameof(DueAmount)] = DueAmount
                };
            }
        }
    }
}
