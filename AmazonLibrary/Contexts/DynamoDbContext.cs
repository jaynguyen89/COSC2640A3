using Amazon;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.Options;

namespace AmazonLibrary.Contexts {

    public sealed class DynamoDbContext {
        
        private readonly IAmazonDynamoDB _dbContext;

        public DynamoDbContext(IOptions<AmazonOptions> options) {
            _dbContext = new AmazonDynamoDBClient(
                options.Value.AwsAccessKeyId,
                options.Value.AwsSecretKey,
                new AmazonDynamoDBConfig {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(options.Value.RegionEndpoint)
                }
            );
        }
        
        public IAmazonDynamoDB GetInstance() {
            return _dbContext;
        }
    }
}