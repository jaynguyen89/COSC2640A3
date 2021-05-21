using Newtonsoft.Json;

namespace AssistantLibrary.Models {

    public sealed class PaypalAccessTokenParser {
        
        public string Scope { get; set; }
        
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        
        [JsonProperty("app_id")]
        public string AppId { get; set; }
        
        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }
        
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
    }

    public sealed class PaypalPaymentAuthorizationParser {
        
       public string Id { get; set; }
       
       public string Intent { get; set; }
       
       public string Status { get; set; }
       
       [JsonProperty("purchase_units")]
       public PurchaseUnit[] PurchaseUnits { get; set; }
       
       public sealed class PurchaseUnit {
           
           public PurchaseAmount Amount { get; set; }
           
           public AuthorizedPayee Payee { get; set; }
           
           [JsonProperty("payments")]
           public PaymentDetail Payment { get; set; }
           
           public sealed class PurchaseAmount {
               
               [JsonProperty("currency_code")]
               public string CurrencyCode { get; set; }
               
               public string Value { get; set; }
           }
           
           public sealed class AuthorizedPayee {
               
               [JsonProperty("email_address")]
               public string Email { get; set; }
               
               [JsonProperty("merchant_id")]
               public string MerchantId { get; set; }
           }
           
           public sealed class PaymentDetail {
               
               public PaymentAuthorization[] Authorizations { get; set; }
               
               public sealed class PaymentAuthorization {
                   
                   public string Status { get; set; }
                   
                   public string Id { get; set; }
                   
                   [JsonProperty("expiration_time")]
                   public string ExpirationTime { get; set; }
               }
           }
       }
    }

    public sealed class PaypalCaptureParser {
        
        public string Id { get; set; }
        
        public string Status { get; set; }
    }
}