using System;
using Amazon;
using Amazon.S3;
using Microsoft.Extensions.Options;

namespace AmazonLibrary.Contexts {

    public sealed class AmazonS3Context {
        
        private readonly IAmazonS3 _s3Context;

        public AmazonS3Context(IOptions<AmazonOptions> options) {
            _s3Context = new AmazonS3Client(
                options.Value.AwsAccessKeyId,
                options.Value.AwsSecretKey,
                new AmazonS3Config {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(options.Value.RegionEndpoint),
                    Timeout = TimeSpan.FromSeconds(int.Parse(options.Value.S3TimeoutSeconds))
                }
            );
        }
        
        public IAmazonS3 GetInstance() {
            return _s3Context;
        }
    }
}