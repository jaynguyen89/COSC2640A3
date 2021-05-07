using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AssistantLibrary.Interfaces;
using AssistantLibrary.Models;
using Google.Authenticator;
using Helper;
using Helper.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AssistantLibrary.Services {

    public sealed class GoogleService : IGoogleService {

        private readonly ILogger<GoogleService> _logger;
        private readonly IOptions<AssistantOptions> _options;
        private readonly HttpClient _httpClient = new();
        private readonly TwoFactorAuthenticator _authenticator;
        
        private string RecaptchaSecretKey { get; set; }

        public GoogleService(
            ILogger<GoogleService> logger,
            IOptions<AssistantOptions> options
        ) {
            _logger = logger;
            _options = options;
            _authenticator = new TwoFactorAuthenticator();

            RecaptchaSecretKey = options.Value.GoogleRecaptchaSecretKey;
            _httpClient.BaseAddress = new Uri(options.Value.GoogleRecaptchaEndpoint);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<GoogleRecaptchaResponse> IsHumanInteraction(string recaptchaToken = null) {
            _logger.LogInformation($"{ nameof(GoogleService) }.{ nameof(IsHumanInteraction) }: Send request to GoogleRecaptcha API.");
            
            if (!bool.Parse(_options.Value.GoogleRecaptchaEnabled)) return new GoogleRecaptchaResponse { Result = true };
            if (recaptchaToken == null) return new GoogleRecaptchaResponse { Result = false };

            var response = await _httpClient.PostAsJsonAsync(
                $"?secret={ RecaptchaSecretKey }&response={ recaptchaToken }",
                HttpCompletionOption.ResponseContentRead
            );

            var verification = new GoogleRecaptchaResponse();
            if (response.IsSuccessStatusCode)
                verification = JsonConvert.DeserializeObject<GoogleRecaptchaResponse>(await response.Content.ReadAsStringAsync());

            return verification;
        }

        public TwoFa ProduceTwoFactorAuthSetup(string email) {
            _logger.LogInformation($"{ nameof(GoogleService) }.{ nameof(ProduceTwoFactorAuthSetup) }: Service starts.");
            
            var secretKey = Helpers.GenerateRandomString(
                Helpers.GetRandomNumberInRangeInclusive(
                    int.Parse(_options.Value.TwoFaSecretLengthMax), int.Parse(_options.Value.TwoFaSecretLengthMin)
                ), 
                true
            );

            var authenticator = _authenticator.GenerateSetupCode(
                SharedConstants.ProjectName, email, secretKey, true, int.Parse(_options.Value.TwoFaQrImageSize)
            );

            return new TwoFa {
                SecretKey = secretKey,
                QrCodeImageUrl = authenticator.QrCodeSetupImageUrl,
                ManualEntryKey = authenticator.ManualEntryKey
            };
        }

        public TwoFa ReproduceTwoFaAuth(string secretKey, string email) {
            _logger.LogInformation($"{ nameof(GoogleService) }.{ nameof(ReproduceTwoFaAuth) }: Service starts.");
            
            var authenticator = _authenticator.GenerateSetupCode(
                SharedConstants.ProjectName, email, secretKey, true, int.Parse(_options.Value.TwoFaQrImageSize)
            );

            return new TwoFa {
                QrCodeImageUrl = authenticator.QrCodeSetupImageUrl,
                ManualEntryKey = authenticator.ManualEntryKey
            };
        }

        public string GetTwoFaPin(string secretKey) {
            _logger.LogInformation($"{ nameof(GoogleService) }.{ nameof(GetTwoFaPin) }: Service starts.");
            return _authenticator.GetCurrentPIN(secretKey);
        }

        public bool VerifyTwoFactorAuth(string secretKey, string pin) {
            _logger.LogInformation($"{ nameof(GoogleService) }.{ nameof(VerifyTwoFactorAuth) }: Service starts.");
            return _authenticator.ValidateTwoFactorPIN(
                secretKey, pin,
                TimeSpan.FromMinutes(int.Parse(_options.Value.TwoFaTolerance))
            );
        }
    }
}