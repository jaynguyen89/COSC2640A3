using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using AmazonLibrary.Contexts;
using AmazonLibrary.Interfaces;
using AmazonLibrary.Models;
using Helper.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AmazonLibrary.Services {

    public sealed class DynamoService : IDynamoService {

        private readonly IOptions<AmazonOptions> _options;
        private readonly ILogger<DynamoService> _logger;
        private readonly IAmazonDynamoDB _dbContext;

        public DynamoService(
            IOptions<AmazonOptions> options,
            ILogger<DynamoService> logger,
            DynamoDbContext dbContext
        ) {
            _options = options;
            _logger = logger;
            _dbContext = dbContext.GetInstance();
        }

        public async Task<string> SaveToSchedulesTable(ImportSchedule schedule, SharedEnums.ImportType importType) {
            _logger.LogInformation($"{ nameof(DynamoService) }.{ nameof(SaveToSchedulesTable) }: Service starts.");
            
            var tableNameToSaveItem = importType == SharedEnums.ImportType.Classroom
                ? _options.Value.ClassroomImportSchedulesTableName
                : _options.Value.StudentImportSchedulesTableName;

            Table tableToSaveItem;
            try {
                tableToSaveItem = Table.LoadTable(_dbContext, tableNameToSaveItem);
            }
            catch (Exception e) {
                _logger.LogError($"{ nameof(DynamoService) }.{ nameof(SaveToSchedulesTable) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }

            var itemDocument = schedule.CreateItemDocument();
            _ = await tableToSaveItem.PutItemAsync(itemDocument);

            return itemDocument[nameof(ImportSchedule.Id)];
        }

        public async Task<ImportSchedule[]> GetAllSchedulesDataFor(string accountId) {
            _logger.LogInformation($"{ nameof(DynamoService) }.{ nameof(GetAllSchedulesDataFor) }: Service starts.");

            var classroomSchedules = await GetSchedulesByTable(_options.Value.ClassroomImportSchedulesTableName, accountId);
            var studentSchedules = await GetSchedulesByTable(_options.Value.StudentImportSchedulesTableName, accountId);

            return classroomSchedules.Concat(studentSchedules).ToArray();
        }

        public async Task<EmrProgress> GetLastEmrProgress() {
            _logger.LogInformation($"{ nameof(DynamoService) }.{ nameof(GetLastEmrProgress) }: Service starts.");

            var scanEmrProgressTableRequest = new ScanRequest { TableName = _options.Value.EmrProgressTableName };

            try {
                var response = await _dbContext.ScanAsync(scanEmrProgressTableRequest);
                if (response.HttpStatusCode != HttpStatusCode.OK) throw new InternalServerErrorException("Scan request to DynamoDB failed.");

                return response.Count == 0
                    ? new EmrProgress()
                    : response.Items
                              .OrderByDescending(item => long.Parse(item[nameof(EmrProgress.Timestamp)].N))
                              .First();
            }
            catch (InternalServerErrorException e) {
                _logger.LogError($"{ nameof(DynamoService) }.{ nameof(GetLastEmrProgress) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (ProvisionedThroughputExceededException e) {
                _logger.LogError($"{ nameof(DynamoService) }.{ nameof(GetLastEmrProgress) } - { nameof(ProvisionedThroughputExceededException) }: { e.StackTrace }");
                return default;
            }
            catch (RequestLimitExceededException e) {
                _logger.LogError($"{ nameof(DynamoService) }.{ nameof(GetLastEmrProgress) } - { nameof(RequestLimitExceededException) }: { e.StackTrace }");
                return default;
            }
            catch (ResourceNotFoundException e) {
                _logger.LogError($"{ nameof(DynamoService) }.{ nameof(GetLastEmrProgress) } - { nameof(ResourceNotFoundException) }: { e.StackTrace }");
                return default;
            }
        }

        public async Task<EmrStatistics[]> GetEmrStatistics() {
            _logger.LogInformation($"{ nameof(DynamoService) }.{ nameof(GetEmrStatistics) }: Service starts.");

            var scanEmrResultsTableRequest = new ScanRequest { TableName = _options.Value.EmrResultsTableName };

            try {
                var response = await _dbContext.ScanAsync(scanEmrResultsTableRequest);
                if (response.HttpStatusCode != HttpStatusCode.OK) throw new InternalServerErrorException("Scan request to DynamoDB failed.");

                return response.Count == 0
                    ? default
                    : response.Items.Select(item => (EmrStatistics) item).ToArray();
            }
            catch (InternalServerErrorException e) {
                _logger.LogError($"{ nameof(DynamoService) }.{ nameof(GetEmrStatistics) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
            catch (ProvisionedThroughputExceededException e) {
                _logger.LogError($"{ nameof(DynamoService) }.{ nameof(GetEmrStatistics) } - { nameof(ProvisionedThroughputExceededException) }: { e.StackTrace }");
                return null;
            }
            catch (RequestLimitExceededException e) {
                _logger.LogError($"{ nameof(DynamoService) }.{ nameof(GetEmrStatistics) } - { nameof(RequestLimitExceededException) }: { e.StackTrace }");
                return null;
            }
            catch (ResourceNotFoundException e) {
                _logger.LogError($"{ nameof(DynamoService) }.{ nameof(GetEmrStatistics) } - { nameof(ResourceNotFoundException) }: { e.StackTrace }");
                return null;
            }
        }

        private async Task<ImportSchedule[]> GetSchedulesByTable(string tableName, string accountId) {
            _logger.LogInformation($"{ nameof(DynamoService) }.{ nameof(GetAllSchedulesDataFor) }: Service starts.");
            
            var scanSchedulesRequest = new ScanRequest {
                TableName = tableName,
                ScanFilter = new Dictionary<string, Condition> {
                    {
                        nameof(ImportSchedule.AccountId),
                        new Condition {
                            ComparisonOperator = ComparisonOperator.EQ,
                            AttributeValueList = new List<AttributeValue> { new(accountId) }
                        }
                    }
                }
            };

            try {
                var response = await _dbContext.ScanAsync(scanSchedulesRequest);
                if (response.HttpStatusCode != HttpStatusCode.OK) throw new InternalServerErrorException("Request to AWS DynamoDb failed.");
                
                return response.Items
                               .Select(item => {
                                   var schedule = (ImportSchedule) item;
                                   schedule.IsForClassroom = tableName.Equals(_options.Value.ClassroomImportSchedulesTableName);
                                   return schedule;
                               })
                               .ToArray();
            }
            catch (InternalServerErrorException e) {
                _logger.LogError($"{ nameof(DynamoService) }.{ nameof(GetAllSchedulesDataFor) } - { nameof(InternalServerErrorException) }: { e.StackTrace }");
                return default;
            }
            catch (ProvisionedThroughputExceededException e) {
                _logger.LogError($"{ nameof(DynamoService) }.{ nameof(GetAllSchedulesDataFor) } - { nameof(ProvisionedThroughputExceededException) }: { e.StackTrace }");
                return default;
            }
            catch (RequestLimitExceededException e) {
                _logger.LogError($"{ nameof(DynamoService) }.{ nameof(GetAllSchedulesDataFor) } - { nameof(RequestLimitExceededException) }: { e.StackTrace }");
                return default;
            }
            catch (ResourceNotFoundException e) {
                _logger.LogError($"{ nameof(DynamoService) }.{ nameof(GetAllSchedulesDataFor) } - { nameof(ResourceNotFoundException) }: { e.StackTrace }");
                return default;
            }
            catch (NullReferenceException) {
                return default;
            }
        }
    }
}