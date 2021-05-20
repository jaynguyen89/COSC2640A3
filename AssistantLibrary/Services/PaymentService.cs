using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using AssistantLibrary.Interfaces;
using AssistantLibrary.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Stripe;

namespace AssistantLibrary.Services {

    public sealed class PaymentService : IPaymentService {
        
        private readonly ILogger<PaymentService> _logger;
        private readonly IOptions<AssistantOptions> _options;
        
        public PaymentService(
            ILogger<PaymentService> logger,
            IOptions<AssistantOptions> options
        ) {
            _logger = logger;
            _options = options;
            StripeConfiguration.ApiKey = _options.Value.StripeSecretKey;
            StripeConfiguration.ClientId = _options.Value.StripeClientId;
        }

        public async Task<bool?> IsPaymentAuthorizationValid(PaypalAuthorization paymentAuthorization) {
            _logger.LogInformation($"{ nameof(PaymentService) }.{ nameof(IsPaymentAuthorizationValid) }: Service starts.");
            var httpClient = new HttpClient();

            var verificationUrl = _options.Value.PaypalVerifyPaymentAuthorizationUrl.Replace(
                _options.Value.PaypalUrlPlaceholder, paymentAuthorization.OrderId
            );
            httpClient.BaseAddress = new Uri(verificationUrl);
            
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var accessToken = await GetPaypalAccessToken();
            if (accessToken is null) return default;
            
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetAsync(string.Empty, HttpCompletionOption.ResponseContentRead);
            if (!response.IsSuccessStatusCode) return default;

            var verification = JsonConvert.DeserializeObject<PaypalPaymentAuthorizationParser>(await response.Content.ReadAsStringAsync());
            if (verification is null) return default;

            return verification.Intent.ToUpper().Equals("AUTHORIZE") &&
                   verification.Status.ToUpper().Equals("COMPLETED") &&
                   verification.PurchaseUnits[0].Amount.CurrencyCode.ToUpper().Equals("AUD") &&
                   decimal.Parse(verification.PurchaseUnits[0].Amount.Value) == paymentAuthorization.Amount &&
                   verification.PurchaseUnits[0].Payee.Email.ToUpper().Equals(_options.Value.PaypalMerchantEmailAddress.ToUpper()) &&
                   verification.PurchaseUnits[0].Payee.MerchantId.ToUpper().Equals(_options.Value.PaypalMerchantId.ToUpper()) &&
                   verification.PurchaseUnits[0].Payment.Authorizations[0].Status.ToUpper().Equals("CREATED") &&
                   DateTime.Parse(verification.PurchaseUnits[0].Payment.Authorizations[0].ExpirationTime) > DateTime.UtcNow;
        }

        public async Task<bool?> CaptureMoneyFromAuthorizedPayment(PaypalAuthorization paymentAuthorization) {
            _logger.LogInformation($"{ nameof(PaymentService) }.{ nameof(CaptureMoneyFromAuthorizedPayment) }: Service starts.");
            var httpClient = new HttpClient();

            var captureUrl = _options.Value.PaypalCapturePaymentUrl.Replace(
                _options.Value.PaypalUrlPlaceholder, paymentAuthorization.AuthorizationId
            );
            httpClient.BaseAddress = new Uri(captureUrl);
            
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var accessToken = await GetPaypalAccessToken();
            if (accessToken is null) return default;
            
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.PostAsJsonAsync(string.Empty, new {});
            if (!response.IsSuccessStatusCode) return default;

            var result = JsonConvert.DeserializeObject<PaypalCaptureParser>(await response.Content.ReadAsStringAsync());
            return result?.Status.ToUpper().Equals("COMPLETED");
        }

        public async Task<KeyValuePair<string, string>> CaptureStripePaymentFrom(StripeAuthorization paymentAuthorization) {
            _logger.LogInformation($"{ nameof(PaymentService) }.{ nameof(CaptureStripePaymentFrom) }: Service starts.");
            var stripeService = new ChargeService();
            var chargeResult = await stripeService.CreateAsync(new ChargeCreateOptions {
                Amount = (long) paymentAuthorization.Details.Amount * 100,
                Capture = true,
                Currency = "AUD",
                Description = $"Payment for { paymentAuthorization.Details.ClassName } ({ paymentAuthorization.Details.ClassroomId })",
                Source = paymentAuthorization.TokenId
            });

            if (!chargeResult.Paid || !chargeResult.Status.ToUpper().Equals("SUCCEEDED") || chargeResult.Amount != (long) paymentAuthorization.Details.Amount * 100)
                return new KeyValuePair<string, string>(default, default);

            return new KeyValuePair<string, string>(chargeResult.BalanceTransactionId, chargeResult.Id);
        }

        private async Task<string> GetPaypalAccessToken() {
            _logger.LogInformation($"private { nameof(GoogleService) }.{ nameof(GetPaypalAccessToken) }: Service starts.");
            var httpClient = new HttpClient {
                BaseAddress = new Uri(_options.Value.PaypalGetAccessTokenUrl)
            };
            
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    Encoding.ASCII.GetBytes($"{ _options.Value.PaypalClientId }:{ _options.Value.PaypalSecret }")
                )
            );

            var req = new HttpRequestMessage(
                HttpMethod.Post,
                _options.Value.PaypalGetAccessTokenUrl
            ) {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>{ new("grant_type", "client_credentials") })
            };
            
            var response = await httpClient.SendAsync(req);
            if (!response.IsSuccessStatusCode) return default;

            var content = JsonConvert.DeserializeObject<PaypalAccessTokenParser>(await response.Content.ReadAsStringAsync());
            return content?.AccessToken;
        }
    }
}