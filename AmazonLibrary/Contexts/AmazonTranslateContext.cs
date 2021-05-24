using Amazon;
using Amazon.Internal;
using Amazon.Translate;
using Microsoft.Extensions.Options;

namespace AmazonLibrary.Contexts {

    public sealed class AmazonTranslateContext {

        private readonly IAmazonTranslate _translateContext;

        public AmazonTranslateContext(IOptions<AmazonOptions> options) {
            _translateContext = new AmazonTranslateClient(
                options.Value.AwsAccessKeyId ?? "AKIAJSENDXCAPZWGB6HQ",
                options.Value.AwsSecretKey ?? "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",
                RegionEndpoint.GetBySystemName(options.Value.RegionEndpoint)
            );
        }

        public IAmazonTranslate GetInstance() {
            return _translateContext;
        }
    }
}