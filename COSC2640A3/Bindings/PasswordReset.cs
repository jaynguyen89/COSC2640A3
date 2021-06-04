using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Helper;
using Helper.Shared;

namespace COSC2640A3.Bindings {

    public sealed class PasswordReset {
        
        public string AccountId { get; set; }
        
        public string RecoveryToken { get; set; }
        
        public string Password { get; set; }
        
        public string PasswordConfirm { get; set; }
        
        public string RecaptchaToken { get; set; }

        public string[] VerifyPasswordReset() {
            var errors = new List<string>();
            
            if (!Helpers.IsProperString(Password)) errors.Add($"{ nameof(Password) } is required.");
            if (!Helpers.IsProperString(PasswordConfirm)) errors.Add($"{ nameof(PasswordConfirm).ToHumanStyled() } is required.");
            
            if (Password.Contains(SharedConstants.MonoSpace)) errors.Add($"{ nameof(Password) } should not contain white space.");
            else {
                var formatTest = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");
                if (!formatTest.IsMatch(Password)) errors.Add($"{ nameof(Password) } requires at least 8 characters long and contains number, upper- and lower-case characters.");
                
                if (!SharedConstants.SpecialChars.Any(Password.Contains)) errors.Add($"{ nameof(Password) } should contains at least 1 special character EXCEPT `single-quote`, `double-quote`, `slashes`, `acute`.");
            }

            if (!Password.Equals(PasswordConfirm)) errors.Add($"{ nameof(Password) } and { nameof(PasswordConfirm).ToHumanStyled() } do not match.");

            return errors.ToArray();
        }
    }
}