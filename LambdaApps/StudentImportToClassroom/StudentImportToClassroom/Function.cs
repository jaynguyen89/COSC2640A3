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

namespace StudentImportToClassroom {

    public class Function {
        
        private readonly IAmazonS3 _s3Service;
        private readonly IAmazonDynamoDB _dynamoService;
        private readonly SqlConnection _dbConnection;

        private const byte ScheduleProcessingStatus = 1;
        private const byte ScheduleDoneStatus = 2;
        private const byte SchedulePartialStatus = 3;
        private const byte ScheduleFailedStatus = 4;
        
        private const string ScheduleTableName = "student.import.schedules";
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
                context.Logger.LogLine($"SQS Event #{ message.MessageId } failed: unable to update get S3 File #{ s3Event.Detail.Asset.FileId }.");
                await Task.CompletedTask;
                return;
            }
            
            var fileContent = await GetContentFrom(fileStream);
            if (fileContent == null) {
                await Terminate(context.Logger, importSchedule, $"SQS Event #{ message.MessageId } failed: unable to get content from file stream.");
                await Task.CompletedTask;
                return;
            }

            var classroomEnrolments = JsonConvert.DeserializeObject<StudentEnrolment[]>(fileContent);
            if (classroomEnrolments == null) {
                await Terminate(context.Logger, importSchedule, $"SQS Event #{ message.MessageId } failed: unable to deserialize file content to { nameof(StudentEnrolment) } array.");
                await Task.CompletedTask;
                return;
            }

            var saveDataResult = classroomEnrolments.Select(classroomEnrolment => {
                    var saveResults = classroomEnrolment.Enrolments.Select(async enrolment => {
                        var savedInvoiceId = await SaveInvoiceIntoDatabase(enrolment.Invoice);

                        var saveEnrolmentResult = false;
                        if (!string.IsNullOrEmpty(savedInvoiceId))
                            saveEnrolmentResult = await SaveEnrolmentIntoDatabase(enrolment, savedInvoiceId, classroomEnrolment.ClassroomId);

                        return saveEnrolmentResult;
                    })
                    .Select(task => task.Result);

                    return saveResults.All(result => result);
                })
                .ToArray();

            if (saveDataResult.All(result => result)) importSchedule.Status = ScheduleDoneStatus;
            else if (saveDataResult.All(result => !result)) importSchedule.Status = ScheduleFailedStatus;
            else importSchedule.Status = SchedulePartialStatus;
            
            _ = await UpdateImportSchedule(importSchedule);
            await Task.CompletedTask;
        }

        private async Task<bool> SaveEnrolmentIntoDatabase(StudentEnrolment.Enrolment enrolment, string invoiceId, string classroomId) {
            try {
                var query = enrolment.GetInsertEnrolmentStatement(invoiceId, classroomId);
                var command = new SqlCommand(query, _dbConnection);
                
                await _dbConnection.OpenAsync();
                var response = await command.ExecuteNonQueryAsync();
                
                await _dbConnection.CloseAsync();
                return response == 1;
            }
            catch (Exception) { return false; }
        }

        private async Task<string> SaveInvoiceIntoDatabase(StudentEnrolment.Enrolment.InvoiceInfo invoice) {
            try {
                var query = invoice.GetInsertInvoiceStatement();
                var command = new SqlCommand(query, _dbConnection);
                
                await _dbConnection.OpenAsync();
                var insertedId = (string) await command.ExecuteScalarAsync();
                        
                await _dbConnection.CloseAsync();
                return insertedId;
            }
            catch (Exception) { return null; }
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

        private async Task Terminate(ILambdaLogger logger, ImportSchedule importSchedule, string message) {
            logger.LogLine(message);
            importSchedule.Status = ScheduleFailedStatus;
            _ = await UpdateImportSchedule(importSchedule);
        }

        public sealed class StudentEnrolment {
            
            public string ClassroomId { get; set; }
            public Enrolment[] Enrolments { get; set; }
            
            public class Enrolment {
                
                public string StudentId { get; set; }
                public string InvoiceId { get; set; }
                public DateTime EnrolledOn { get; set; }
                public byte? OverallMark { get; set; }
                public string MarkBreakdowns { get; set; }
                public bool? IsPassed { get; set; }
                public InvoiceInfo Invoice { get; set; }

                public string GetInsertEnrolmentStatement(string invoiceId, string classroomId) {
                    return $"INSERT INTO Invoice (" +
                               $"{ nameof(StudentId) }, " +
                               $"{ nameof(ClassroomId) }, " +
                               $"{ nameof(InvoiceId) }, " +
                               $"{ nameof(EnrolledOn) }, " +
                               $"{ nameof(OverallMark) }, " +
                               $"{ nameof(MarkBreakdowns) }, " +
                               $"{ nameof(IsPassed) }, " +
                               ") " +
                           "VALUES (" +
                               $"{ StudentId }, " +
                               $"{ classroomId }, " +
                               $"{ invoiceId }, " +
                               $"{ EnrolledOn }, " +
                               $"{ OverallMark }, " +
                               $"{ MarkBreakdowns }, " +
                               $"{ IsPassed }, " +
                           ");";
                }

                public class InvoiceInfo {
                    
                    public decimal DueAmount { get; set; }
                    public bool IsPaid { get; set; }
                    public string PaymentMethod { get; set; }
                    public string PaymentId { get; set; }
                    public string TransactionId { get; set; }
                    public string ChargeId { get; set; }
                    public string PaymentStatus { get; set; }
                    public DateTime? PaidOn { get; set; }

                    public string GetInsertInvoiceStatement() {
                        return $"INSERT INTO Invoice (" +
                                   $"{ nameof(DueAmount) }, " +
                                   $"{ nameof(IsPaid) }, " +
                                   $"{ nameof(PaymentMethod) }, " +
                                   $"{ nameof(PaymentId) }, " +
                                   $"{ nameof(TransactionId) }, " +
                                   $"{ nameof(ChargeId) }, " +
                                   $"{ nameof(PaymentStatus) }, " +
                                   $"{ nameof(PaidOn) }, " +
                               ") OUTPUT INSERTED.Id " +
                               "VALUES (" +
                                   $"{ DueAmount }, " +
                                   $"{ IsPaid }, " +
                                   $"{ PaymentMethod }, " +
                                   $"{ PaymentId }, " +
                                   $"{ TransactionId }, " +
                                   $"{ ChargeId }, " +
                                   $"{ PaymentStatus }, " +
                                   $"{ PaidOn }, " +
                               ");";
                    }
                }
            }
        }
    }
}
