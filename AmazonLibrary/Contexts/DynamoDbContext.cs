using Amazon;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.Options;

namespace AmazonLibrary.Contexts {

    public sealed class DynamoDbContext {
        
        private readonly IAmazonDynamoDB _dbContext;

        public DynamoDbContext(IOptions<AmazonOptions> options) {
            _dbContext = new AmazonDynamoDBClient(
                options.Value.AwsAccessKeyId ?? "AKIAJSENDXCAPZWGB6HQ",
                options.Value.AwsSecretKey ?? "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",
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