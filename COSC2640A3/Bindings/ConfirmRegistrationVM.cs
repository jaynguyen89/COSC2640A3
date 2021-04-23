using System;
using System.Collections.Generic;
using Helper;

namespace COSC2640A3.Bindings {

    public sealed class ConfirmRegistrationVM {
        
        public string Email { get; set; }
        
        public string Username { get; set; }
        
        public string ConfirmCode { get; set; }

        public string[] VerifyConfirmation() {
            var errors = new List<string>();
            if ((!Helpers.IsProperString(Email) && !Helpers.IsProperString(Username)) || !Helpers.IsProperString(ConfirmCode))
                errors.Add($"{ nameof(Email) }/{ nameof(Username) } and { nameof(ConfirmCode) } are required.");
            
            Email = Email?.Trim().ToLower().RemoveAllSpaces();
            Username = Username?.Trim().RemoveAllSpaces();
            ConfirmCode = ConfirmCode?.Trim();

            return errors.ToArray();
        }
    }
}