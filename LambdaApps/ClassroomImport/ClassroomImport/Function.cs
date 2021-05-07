using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.S3;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ClassroomImport {

    public class Function {

        private readonly IAmazonS3 _s3Service;
        private readonly IAmazonDynamoDB _dynamoService;
        private readonly SqlConnection _dbConnection;

        private const byte ScheduleProcessingStatus = 1;
        private const byte ScheduleDoneStatus = 2;
        private const byte ScheduleFailedStatus = 4;
        
        private const string ScheduleTableName = "classroom.import.schedules";
        private const string DbConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=COSC2640A3;User Id=something;Password=something;Trusted_Connection=True;";

        public Function() {
            _s3Service = new AmazonS3Client(
                "AKIAJSENDXCAPZWGB6HQ",
                "HeGULGolRgnxwKIIm4K2d8E+sAoHVBukvR+5umU3",
                new AmazonS3Config {
                    RegionEndpoint = RegionEndpoint.APSoutheast2,
                    Timeout = TimeSpan.FromSeconds(30)
                }
            );

            _dynamoService = new AmazonDynamoDBClient(
                "AKIAJSENDXCAPZWGB6HQ",
                "HeGULGolRgnxwKIIm4K2d8E+sAoHVBukvR+5umU3",
                new AmazonDynamoDBConfig {
                    RegionEndpoint = RegionEndpoint.APSoutheast2
                }
            );

            _dbConnection = new SqlConnection(DbConnectionString);
        }

        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context) {
            foreach(var message in evnt.Records)
                await ProcessMessageAsync(message, context);
        }

        private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context) {
            context.Logger.LogLine($"Start processing SQS event #{ message.MessageId }: { message.EventSourceArn }");

            var s3Event = JsonConvert.DeserializeObject<S3Record>(message.Body)?.Records[0];
            if (s3Event == null) {
                context.Logger.LogLine($"SQS Event #{ message.MessageId } failed: missing body.");
                await Task.CompletedTask;
                return;
            }

            var importSchedule = await GetImportScheduleFor(s3Event.Detail.Asset.FileId);
            if (importSchedule == null) {
                context.Logger.LogLine($"SQS Event #{ message.MessageId } failed: unable to get { nameof(ImportSchedule) } from { ScheduleTableName }.");
                await Task.CompletedTask;
                return;
            }

            importSchedule.Status = ScheduleProcessingStatus;
            var updateScheduleResult = await UpdateImportSchedule(importSchedule);
            if (!updateScheduleResult.HasValue || !updateScheduleResult.Value) {
                await Terminate(context.Logger, importSchedule, $"SQS Event #{ message.MessageId } failed: unable to update { nameof(ImportSchedule) } to { ScheduleTableName }.");
                await Task.CompletedTask;
                return;
            }
            
            var fileStream = await GetFileFromS3BucketFor(s3Event.Detail.Asset.FileId, s3Event.Detail.Bucket.Name);
            if (fileStream == Stream.Null) {
                await Terminate(context.Logger, importSchedule, $"SQS Event #{ message.MessageId } failed: unable to update get S3 File #{ s3Event.Detail.Asset.FileId }.");
                await Task.CompletedTask;
                return;
            }
            
            var fileContent = await GetContentFrom(fileStream);
            if (fileContent == null) {
                await Terminate(context.Logger, importSchedule, $"SQS Event #{ message.MessageId } failed: unable to get content from file stream.");
                await Task.CompletedTask;
                return;
            }

            var classrooms = JsonConvert.DeserializeObject<Classroom[]>(fileContent);
            if (classrooms == null) {
                await Terminate(context.Logger, importSchedule, $"SQS Event #{ message.MessageId } failed: unable to deserialize file content to { nameof(Classroom) } array.");
                await Task.CompletedTask;
                return;
            }

            var teacherId = await GetTeacherIdBy(importSchedule.AccountId);
            if (string.IsNullOrEmpty(teacherId)) {
                await Terminate(context.Logger, importSchedule, $"SQS Event #{ message.MessageId } failed: unable to get teacher ID from RDS.");
                await Task.CompletedTask;
                return;
            }

            foreach (var classroom in classrooms) classroom.TeacherId = teacherId;
            var saveDataResult = SaveClassroomsIntoDatabase(classrooms);
            
            importSchedule.Status = saveDataResult ? ScheduleDoneStatus : ScheduleFailedStatus;
            _ = await UpdateImportSchedule(importSchedule);

            await Task.CompletedTask;
        }

        private bool SaveClassroomsIntoDatabase(Classroom[] classrooms) {
            try {
                var results = classrooms.Select(classroom => {
                    var query = classroom.GetInsertStatement();
                    var command = new SqlCommand(query, _dbConnection);

                    try {
                        _dbConnection.Open();
                        var response = command.ExecuteNonQuery();
                        
                        _dbConnection.Close();
                        return response == 1;
                    }
                    catch (Exception) { return false; }
                });

                return results.All(result => result);
            }
            catch (Exception) { return false; }
        }

        private async Task<string> GetTeacherIdBy(string accountId) {
            try {
                const string query = "SELECT Id FROM Teacher WHERE AccountId = @AccountId;";

                var command = new SqlCommand(query, _dbConnection);
                command.Parameters.AddWithValue("@AccountId", accountId);

                await _dbConnection.OpenAsync();
                var reader = await command.ExecuteReaderAsync();
                
                var teacherId = string.Empty;
                if (await reader.ReadAsync()) teacherId = reader["Id"].ToString();

                await _dbConnection.CloseAsync();
                return teacherId;
            }
            catch (Exception) { return default; }
        }

        private async Task<string> GetContentFrom(Stream fileStream) {
            try {
                var reader = new StreamReader(fileStream);
                return await reader.ReadToEndAsync();
            }
            catch (Exception) { return default; }
        }

        private async Task<Stream> GetFileFromS3BucketFor(string fileId, string bucketName) {
            try {
                var response = await _s3Service.GetObjectAsync(bucketName, fileId);
                if (response.HttpStatusCode != HttpStatusCode.OK) throw new InternalServerErrorException("Request to AWS S3 failed.");

                var fileStream = new MemoryStream();
                await response.ResponseStream.CopyToAsync(fileStream);

                return fileStream;
            }
            catch (Exception) { return Stream.Null; }
        }

        private async Task<ImportSchedule> GetImportScheduleFor(string fileId) {
            try {
                var scanImportScheduleTableRequest = new ScanRequest {
                    TableName = ScheduleTableName,
                    ScanFilter = new Dictionary<string, Condition> {
                        {
                            nameof(ImportSchedule.FileId),
                            new Condition {
                                ComparisonOperator = ComparisonOperator.EQ,
                                AttributeValueList = new List<AttributeValue> { new AttributeValue(fileId) }
                            }
                        },
                        {
                            nameof(ImportSchedule.Status),
                            new Condition {
                                ComparisonOperator = ComparisonOperator.LT,
                                AttributeValueList = new List<AttributeValue> { new AttributeValue(ScheduleProcessingStatus.ToString()) }
                            }
                        }
                    }
                };
                
                var response = await _dynamoService.ScanAsync(scanImportScheduleTableRequest);
                if (response.HttpStatusCode != HttpStatusCode.OK) throw new InternalServerErrorException("Request to AWS DynamoDb failed.");
                
                return response.Count == 0 ? default : response.Items.First();
            }
            catch (Exception) { return default; }
        }

        private async Task<bool?> UpdateImportSchedule(ImportSchedule schedule) {
            try {
                var updateImportScheduleRequest = new UpdateItemRequest {
                    TableName = ScheduleTableName,
                    Key = new Dictionary<string, AttributeValue> {
                        {nameof(ImportSchedule.Id), new AttributeValue(schedule.Id)}
                    },
                    AttributeUpdates = new Dictionary<string, AttributeValueUpdate> {
                        {
                            nameof(ImportSchedule.Status),
                            new AttributeValueUpdate {
                                Action = AttributeAction.PUT,
                                Value = new AttributeValue { N = schedule.Status.ToString() }
                            }
                        }
                    }
                };

                var response = await _dynamoService.UpdateItemAsync(updateImportScheduleRequest);
                return response.HttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception) { return default; }
        }
        
        private async Task Terminate(ILambdaLogger logger, ImportSchedule importSchedule, string message) {
            logger.LogLine(message);
            importSchedule.Status = ScheduleFailedStatus;
            _ = await UpdateImportSchedule(importSchedule);
        }

        public class S3Record {
            public S3Event[] Records { get; set; }
            
            public class S3Event {
                [JsonProperty("awsRegion")]
                public string Region { get; set; }
                [JsonProperty("s3")]
                public EventDetail Detail { get; set; }

                public class EventDetail {
                    
                    public S3Bucket Bucket { get; set; }
                    [JsonProperty("object")]
                    public AssetInfo Asset { get; set; }
                    
                    public class S3Bucket {
                        public string Name { get; set; }
                    }
                    
                    public class AssetInfo {
                        [JsonProperty("key")]
                        public string FileId { get; set; }
                    }
                }
            }
        }
        
        public sealed class ImportSchedule {
        
            [DynamoDBHashKey]
            public string Id { get; set; }
            
            public string AccountId { get; set; }
        
            public string FileId { get; set; }
        
            public byte Status { get; set; }

            public static implicit operator ImportSchedule(Dictionary<string, AttributeValue> item) {
                return new ImportSchedule {
                    Id = item[nameof(Id)].S,
                    AccountId = item[nameof(AccountId)].S,
                    FileId = item[nameof(FileId)].S,
                    Status = byte.Parse(item[nameof(Status)].N)
                };
            }
        }
        
        public sealed class Classroom {
            public string TeacherId { get; set; }
            public string ClassName { get; set; }
            public short Capacity { get; set; }
            public decimal Price { get; set; }
            public DateTime? CommencedOn { get; set; }
            public byte Duration { get; set; }
            public byte DurationUnit { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedOn { get; set; }

            public string GetInsertStatement() {
                return $"INSERT INTO Classroom (" +
                           $"{ nameof(TeacherId) }, " +
                           $"{ nameof(ClassName) }, " +
                           $"{ nameof(Capacity) }, " +
                           $"{ nameof(Price) }, " +
                           $"{ nameof(CommencedOn) }, " +
                           $"{ nameof(Duration) }, " +
                           $"{ nameof(DurationUnit) }, " +
                           $"{ nameof(IsActive) }, " +
                           $"{ nameof(CreatedOn) }" +
                        ") " +
                        "VALUES (" +
                           $"{ TeacherId }, " +
                           $"{ ClassName }, " +
                           $"{ Capacity }, " +
                           $"{ Price }, " +
                           $"{ CommencedOn }, " +
                           $"{ Duration }, " +
                           $"{ DurationUnit }, " +
                           $"{ IsActive }, " +
                           $"{ CreatedOn }" +
                        ");";
            }
        }
    }
}
