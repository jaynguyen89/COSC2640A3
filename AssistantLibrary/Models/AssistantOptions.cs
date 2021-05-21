namespace AssistantLibrary.Models {

    public class AssistantOptions {
        
        // For Google Recaptcha
        public string GoogleRecaptchaEnabled { get; set; }

        public string GoogleRecaptchaEndpoint { get; set; }
        
        public string GoogleRecaptchaSecretKey { get; set; }
        
        // For Google Two Factor Authentication
        public string TwoFaQrImageSize { get; set; }
        
        public string TwoFaSecretLengthMin { get; set; }
        
        public string TwoFaSecretLengthMax { get; set; }
        
        public string TwoFaTolerance { get; set; }
        
        // For SMS service
        public string SmsHttpEndpoint { get; set; }
        
        public string SmsApiKey { get; set; }
        
        public string ApiKeyPlaceholder { get; set; }
        
        public string SmsContentPlaceholder { get; set; }
        
        // For Paypal service
        public string PaypalClientId { get; set; }
        
        public string PaypalSecret { get; set; }
        
        public string PaypalCapturePaymentUrl { get; set; }
        
        public string PaypalGetAccessTokenUrl { get; set; }
        
        public string PaypalVerifyPaymentAuthorizationUrl { get; set; }
        
        public string PaypalUrlPlaceholder { get; set; }
        
        public string PaypalMerchantEmailAddress { get; set; }
        
        public string PaypalMerchantId { get; set; }
        
        // For Stripe service: Google Pay and Card
        public string StripeSecretKey { get; set; }
        
        public string StripeClientId { get; set; }
    }
}