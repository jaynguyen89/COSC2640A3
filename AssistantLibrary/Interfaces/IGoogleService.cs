using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AssistantLibrary.Models;
using Helper.Shared;

namespace AssistantLibrary.Interfaces {

    public interface IGoogleService {
        
        Task<GoogleRecaptchaResponse> IsHumanInteraction([AllowNull] string recaptchaToken = null);
        
        /// <summary>
        /// When user is enabling (or renewing) TFA, generate new TFA QR data for them.
        /// </summary>
        TwoFa ProduceTwoFactorAuthSetup([NotNull] string email);

        /// <summary>
        /// When user already have TFA enabled, get their current TFA QR data for them.
        /// </summary>
        TwoFa ReproduceTwoFaAuth([NotNull] string secretKey,[NotNull] string email);

        /// <summary>
        /// Get Two FA 6-digit PIN to send to user via SMS and Email.
        /// </summary>
        string GetTwoFaPin([NotNull] string secretKey);

        bool VerifyTwoFactorAuth([NotNull] string secretKey,[NotNull] string pin);
    }
}