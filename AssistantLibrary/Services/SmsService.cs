using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AssistantLibrary.Interfaces;
using AssistantLibrary.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AssistantLibrary.Services {

    public sealed class SmsService : ISmsService {

        private readonly ILogger<SmsService> _logger;
        private readonly IOptions<AssistantOptions> _options;
        private readonly HttpClient _httpClient = new();
        
        private string SmsEndpoint { get; set; }

        public SmsService(
            ILogger<SmsService> logger,
            IOptions<AssistantOptions> options
        ) {
            _logger = logger;
            _options = options;
            
            SmsEndpoint = options.Value.SmsHttpEndpoint;
            SmsEndpoint = SmsEndpoint.Replace(options.Value.ApiKeyPlaceholder, options.Value.SmsApiKey);
        }


        public async Task<bool?> SendSmsWithContent(string content) {
            try {
                _logger.LogInformation($"{ nameof(SmsService) }.{ nameof(SendSmsWithContent) }: Service starts.");
                SmsEndpoint = SmsEndpoint.Replace(_options.Value.SmsContentPlaceholder, content);

                _httpClient.BaseAddress = new Uri(SmsEndpoint);
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.GetAsync(string.Empty);
                return response.IsSuccessStatusCode;
            }
            catch (InvalidOperationException e) {
                _logger.LogError($"{ nameof(SmsService) }.{ nameof(SendSmsWithContent) } - { nameof(InvalidOperationException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (HttpRequestException e) {
                _logger.LogError($"{ nameof(SmsService) }.{ nameof(SendSmsWithContent) } - { nameof(HttpRequestException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (TaskCanceledException e) {
                _logger.LogError($"{ nameof(SmsService) }.{ nameof(SendSmsWithContent) } - { nameof(TaskCanceledException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }
    }
}