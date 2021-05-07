using System.Net.Http;
using AssistantLibrary.Interfaces;
using AssistantLibrary.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AssistantLibrary.Services {

    public sealed class PaypalService : IPaypalService {
        
        private readonly ILogger<PaypalService> _logger;
        private readonly IOptions<AssistantOptions> _options;
        private readonly HttpClient _httpClient = new();

        public PaypalService(
            ILogger<PaypalService> logger,
            IOptions<AssistantOptions> options
        ) {
            _logger = logger;
            _options = options;
        }
        
        
    }
}