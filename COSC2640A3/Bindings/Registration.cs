using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Helper;
using Helper.Shared;

namespace COSC2640A3.Bindings {

    public sealed class Registration {
        
        public string Email { get; set; }
        
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public string PasswordConfirm { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public string PreferredName { get; set; }
        
        public string RecaptchaToken { get; set; }

        public string[] VerifyRegistrationDetails() {
            var errors = new List<string>();
            
            if (!Helpers.IsProperString(Email)) errors.Add($"{ nameof(Email) } is required.");
            if (!Helpers.IsProperString(Username)) errors.Add($"{ nameof(Username) } is required.");
            if (!Helpers.IsProperString(Password)) errors.Add($"{ nameof(Password) } is required.");
            if (!Helpers.IsProperString(PasswordConfirm)) errors.Add($"{ nameof(PasswordConfirm).ToHumanStyled() } is required.");

            if (errors.Count != 0) return errors.ToArray();

            if (!Helpers.IsProperString(PhoneNumber)) PhoneNumber = null;
            if (!Helpers.IsProperString(PreferredName)) PreferredName = null;

            Email = Email.Trim().ToLower().RemoveAllSpaces();
            Username = Username.Trim().RemoveAllSpaces();
            Password = Password.Trim();
            PasswordConfirm = PasswordConfirm.Trim();

            var formatTest = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (!formatTest.IsMatch(Email)) errors.Add($"{ nameof(Email) } format seems to be invalid.");
            
            var lengthTest = new Regex(@".{10,100}");
            if (!lengthTest.IsMatch(Email)) errors.Add($"{ nameof(Email) } is too { (Email.Length < 10 ? "short." : "long.") }");
            
            if (SharedConstants.InvalidTokens.Any(Email.Contains)) errors.Add($"{ nameof(Email) } should not contain adjacent special characters.");
            if (SharedConstants.InvalidEnds.Any(Email.StartsWith) || SharedConstants.InvalidEnds.Any(Email.EndsWith))
                errors.Add($"{ nameof(Email) } should not start or end with special characters.");
            
            formatTest = new Regex(@"^[0-9A-Za-z_\-.]*$");
            if (!formatTest.IsMatch(Username)) errors.Add($"{ nameof(Username) } should be alphanumeric and able to have `underscore`, `hyphen`, `period` characters.");
            
            lengthTest = new Regex(@".{6,50}");
            if (!lengthTest.IsMatch(Username)) errors.Add($"{ nameof(Username) } is too { (Username.Length < 6 ? "short." : "long.") }");
            
            if (SharedConstants.InvalidTokens.Any(Username.Contains)) errors.Add($"{ nameof(Username) } should not contain adjacent special characters.");
            if (SharedConstants.InvalidEnds.Any(Username.StartsWith) || SharedConstants.InvalidEnds.Any(Username.EndsWith))
                errors.Add($"{ nameof(Username) } should not start or end with special characters.");
            
            if (Password.Contains(SharedConstants.MonoSpace)) errors.Add($"{ nameof(Password) } should not contain white space.");
            else {
                formatTest = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");
                if (!formatTest.IsMatch(Password)) errors.Add($"{ nameof(Password) } requires at least 8 characters long and contains number, upper- and lower-case characters.");
                
                if (!SharedConstants.SpecialChars.Any(Password.Contains)) errors.Add($"{ nameof(Password) } should contains at least 1 special character EXCEPT `single-quote`, `double-quote`, `slashes`, `acute`.");
            }

            if (!Password.Equals(PasswordConfirm)) errors.Add($"{ nameof(Password) } and { nameof(PasswordConfirm).ToHumanStyled() } do not match.");

            if (errors.Count != 0) return errors.ToArray();
            
            PhoneNumber = PhoneNumber?.Trim();
            PreferredName = PreferredName?.Trim();

            if (Helpers.IsProperString(PhoneNumber)) PhoneNumber = Regex.Replace(PhoneNumber!, SharedConstants.MultiSpace, SharedConstants.MonoSpace);
            if (Helpers.IsProperString(PreferredName)) PreferredName = Regex.Replace(PreferredName!, SharedConstants.MultiSpace, SharedConstants.MonoSpace);

            return Array.Empty<string>();
        }
    }
}