using AssistantLibrary.Models;

namespace COSC2640A3.ViewModels.Account {

    public sealed class TwoFaVM {
        
        public string QrImageUrl { get; set; }
        
        public string ManualQrCode { get; set; }

        public static implicit operator TwoFaVM(TwoFa entry) {
            return new() {
                QrImageUrl = entry.QrCodeImageUrl,
                ManualQrCode = entry.ManualEntryKey
            };
        }
    }
}