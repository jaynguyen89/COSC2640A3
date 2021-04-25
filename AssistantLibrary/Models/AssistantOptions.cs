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
    }
}