using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AssistantLibrary.Models;
using Helper.Shared;

namespace AssistantLibrary.Interfaces {

    public interface IGoogleService {
        
        Task<GoogleRecaptchaResponse> IsHumanRegistration([AllowNull] string recaptchaToken = null);
        
        TwoFa GetTwoFactorAuthSetup(
            [NotNull] string email,
            [NotNull] int imageSize,
            [NotNull] string projectName = SharedConstants.ProjectName
        );

        bool VerifyTwoFactorAuth([NotNull] string secretKey,[NotNull] string pin);
    }
}