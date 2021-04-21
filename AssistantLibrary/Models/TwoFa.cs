namespace AssistantLibrary.Models {

    public sealed class TwoFa {
        
        public string SecretKey { get; set; }
        
        public string QrCodeImageUrl { get; set; }
        
        public string ManualEntryKey { get; set; }
    }
}