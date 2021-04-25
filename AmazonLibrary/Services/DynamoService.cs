using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using AmazonLibrary.Contexts;
using AmazonLibrary.Interfaces;
using AmazonLibrary.Models;
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

        public async Task<string> SaveToSchedulesTable(ImportSchedule schedule, string importType) {
            _logger.LogInformation($"{ nameof(DynamoService) }.{ nameof(SaveToSchedulesTable) }: Service starts.");
            
            var tableNameToSaveItem = importType.Equals("Classroom")
                ? _options.Value.ClassroomImportSchedulesTableName
                : (importType.Equals("Students")
                    ? _options.Value.StudentImportSchedulesTableName
                    : _options.Value.ClassroomAndStudentImportScheduleTableName
                );

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
    }
}