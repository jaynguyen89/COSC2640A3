using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace DataReducer {

    public sealed class Program {

        private readonly IAmazonDynamoDB _dynamoService;
        private readonly IAmazonS3 _s3Service;

        private const string MapReduceDataBucket = "cosc2640a3.map.reduce.data";
        private const string MapReduceDataTable = "cosc2640a3.map.reduce.data";
        private const string MapReduceResultsTable = "cosc2640a3.map.reduce.results";
        private const string MapReduceProgressTable = "cosc2640a3.map.reduce.progress";

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
        }

        public static async Task Main(string[] args) {
            var program = new Program();

            var dataFileId = await GetLastDataFileId(program._dynamoService);
            if (dataFileId == null) Environment.Exit(-1);

            var progressId = await GetLastProgress(program._dynamoService);
            if (progressId == null) Environment.Exit(-1);

            var data = await GetDataFromS3FileByKey(dataFileId, program._s3Service);
            if (data == null || data.Length == 0) Environment.Exit(-1);

            var range50Counts = 0;
            var range100Counts = 0;
            var range250Counts = 0;
            var range500Counts = 0;
            var range1000Counts = 0;
            var range1001Counts = 0;

            foreach (var item in data) {
                if (item.DueAmount <= 50) range50Counts++;
                if (item.DueAmount > 50 && item.DueAmount <= 100) range100Counts++;
                if (item.DueAmount > 100 && item.DueAmount <= 250) range250Counts++;
                if (item.DueAmount > 250 && item.DueAmount <= 500) range500Counts++;
                if (item.DueAmount > 500 && item.DueAmount <= 1000) range1000Counts++;
                if (item.DueAmount > 1000) range1001Counts++;
            }

            var total = data.Length;
            var stats = new {
                Range50 = Math.Round(range50Counts * 100.0 / total, 2),
                Range100 = Math.Round(range100Counts * 100.0 / total, 2),
                Range250 = Math.Round(range250Counts * 100.0 / total, 2),
                Range500 = Math.Round(range500Counts * 100.0 / total, 2),
                Range1000 = Math.Round(range1000Counts * 100.0 / total, 2),
                Range1001 = Math.Round(range1001Counts * 100.0 / total, 2)
            };

            await SaveResult(JsonConvert.SerializeObject(stats), program._dynamoService);
            await UpdateProgress(progressId, program._dynamoService);
        }

        private static async Task UpdateProgress(string progressId, IAmazonDynamoDB dynamoService) {
            var updateItemRequest = new UpdateItemRequest {
                TableName = MapReduceProgressTable,
                Key = new Dictionary<string, AttributeValue> {
                    { "Id", new AttributeValue { S = progressId } }
                },
                AttributeUpdates = new Dictionary<string, AttributeValueUpdate> {
                    ["ReducerDone"] = new AttributeValueUpdate {
                        Action = AttributeAction.PUT,
                        Value = new AttributeValue { BOOL = true }
                    },
                    ["Timestamp"] = new AttributeValueUpdate {
                        Action = AttributeAction.PUT,
                        Value = new AttributeValue { N = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() }
                    }
                },
                ReturnValues = ReturnValue.NONE
            };
            
            _ = await dynamoService.UpdateItemAsync(updateItemRequest);
        }

        private static async Task<string> GetLastProgress(IAmazonDynamoDB dynamoService) {
            var scanEmrProgressTableRequest = new ScanRequest { TableName = MapReduceProgressTable };
            
            try {
                var response = await dynamoService.ScanAsync(scanEmrProgressTableRequest);
                if (response.HttpStatusCode != HttpStatusCode.OK) throw new InternalServerErrorException("Scan request to DynamoDB failed.");

                return response.Count == 0
                    ? default
                    : response.Items
                              .OrderByDescending(item => long.Parse(item["Timestamp"].N))
                              .First()["Id"].S;
            }
            catch (Exception) { return default; }
        }

        private static async Task SaveResult(string result, IAmazonDynamoDB dynamoService) {
            var resultTable = Table.LoadTable(dynamoService, MapReduceResultsTable);
            await resultTable.PutItemAsync(new Document {
                ["Id"] = Guid.NewGuid().ToString().ToLower(),
                ["Statistics"] = result,
                ["Timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        private static async Task<EnrolmentInvoice[]> GetDataFromS3FileByKey(string fileId, IAmazonS3 s3Service) {
            try {
                var getFileRequest = new GetObjectRequest {
                    BucketName = MapReduceDataBucket,
                    Key = fileId
                };

                var response = await s3Service.GetObjectAsync(getFileRequest);
                if (response.HttpStatusCode != HttpStatusCode.OK) throw new InternalServerErrorException("Get file from S3 request failed.");

                var reader = new StreamReader(response.ResponseStream);
                var content = await reader.ReadToEndAsync();

                var data = JsonConvert.DeserializeObject<EnrolmentInvoice[]>(content);
                
                reader.Close();
                return data;
            }
            catch (Exception) { return null; }
        }

        private static async Task<string> GetLastDataFileId(IAmazonDynamoDB dynamoService) {
            try {
                var scanAllRequest = new ScanRequest { TableName = MapReduceDataTable };

                var response = await dynamoService.ScanAsync(scanAllRequest);
                if (response.HttpStatusCode != HttpStatusCode.OK) throw new InternalServerErrorException("Request to AWS DynamoDb failed.");

                return response.Count == 0
                    ? default
                    : response.Items
                              .OrderByDescending(item => long.Parse(item["Timestamp"].N))
                              .First()["FileId"].S;
            }
            catch (Exception) { return default; }
        }

        public sealed class EnrolmentInvoice {
            [DynamoDBHashKey]
            public string Id { get; set; }
            public double DueAmount { get; set; }

            public static implicit operator EnrolmentInvoice(Dictionary<string, AttributeValue> item) {
                return new EnrolmentInvoice {
                    Id = item[nameof(Id)].S,
                    DueAmount = double.Parse(item[nameof(DueAmount)].N)
                };
            }
        }
    }
}
