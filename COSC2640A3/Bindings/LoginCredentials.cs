using System;
using Helper;

namespace COSC2640A3.Bindings {

    public sealed class LoginCredentials : Identity {
        
        public string Password { get; set; }
        
        public bool AsStudent { get; set; }

        public string[] VerifyCredentials() {
            if ((!Helpers.IsProperString(Email) && !Helpers.IsProperString(Username)) || !Helpers.IsProperString(Password))
                return new[] { $"{ nameof(Email) }/{ nameof(Username) } and { nameof(Password) } must be provided." };

            Email = Email?.Trim().ToLower().RemoveAllSpaces();
            Username = Username?.Trim().RemoveAllSpaces();
            Password = Password.Trim();

            return Array.Empty<string>();
        }
    }
}