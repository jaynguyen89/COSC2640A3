using Amazon;
using Amazon.SimpleEmail;
using AmazonLibrary.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AmazonLibrary.Services {

    public sealed class AmazonMailService : IAmazonMailService {

        private readonly ILogger<AmazonMailService> _logger;
        private readonly AmazonSimpleEmailServiceClient _emailService;

        private readonly string FromAddress;

        public AmazonMailService(
            ILogger<AmazonMailService> logger,
            IOptions<AmazonOptions> options
        ) {
            _logger = logger;
            FromAddress = options.Value.MailSentFromAddress;

            var region = RegionEndpoint.GetBySystemName(options.Value.RegionEndpoint);
            _emailService = new AmazonSimpleEmailServiceClient(region);
        }
    }
}