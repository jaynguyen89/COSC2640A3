using Amazon;
using Amazon.Textract;
using Microsoft.Extensions.Options;

namespace AmazonLibrary.Contexts {

    public sealed class AmazonTextractContext {

        private readonly IAmazonTextract _textractContext;

        public AmazonTextractContext(IOptions<AmazonOptions> options) {
            _textractContext = new AmazonTextractClient(
                options.Value.AwsAccessKeyId ?? "AKIAJSENDXCAPZWGB6HQ",
                options.Value.AwsSecretKey ?? "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",
                RegionEndpoint.GetBySystemName(options.Value.RegionEndpoint)
            );
        }

        public IAmazonTextract GetInstance() {
            return _textractContext;
        }
    }
}