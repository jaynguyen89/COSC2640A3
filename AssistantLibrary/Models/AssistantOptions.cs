namespace AssistantLibrary.Models {

    public class AssistantOptions {
        
        public string GoogleRecaptchaEnabled { get; set; }

        public string GoogleRecaptchaEndpoint { get; set; }
        
        public string GoogleRecaptchaSecretKey { get; set; }
        
        public string TwoFaQrImageSize { get; set; }
        
        public string TwoFaSecretLengthMin { get; set; }
        
        public string TwoFaSecretLengthMax { get; set; }
        
        public string TwoFaTolerance { get; set; }
    }
}