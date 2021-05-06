using Amazon;
using Amazon.Textract;
using Microsoft.Extensions.Options;

namespace AmazonLibrary.Contexts {

    public sealed class AmazonTextractContext {

        private readonly IAmazonTextract _textractContext;

        public AmazonTextractContext(IOptions<AmazonOptions> options) {
            _textractContext = new AmazonTextractClient(
                options.Value.AwsAccessKeyId,
                options.Value.AwsSecretKey,
                RegionEndpoint.GetBySystemName(options.Value.RegionEndpoint)
            );
        }

        public IAmazonTextract GetInstance() {
            return _textractContext;
        }
    }
}