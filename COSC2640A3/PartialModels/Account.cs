using COSC2640A3.Bindings;

namespace COSC2640A3.Models {

    public partial class Account {

        public static implicit operator Account(RegistrationVM registration) {
            return new() {
                EmailAddress = registration.Email,
                Username = registration.Username,
                NormalizedUsername = registration.Username.ToUpper(),
                PhoneNumber = registration.PhoneNumber,
                PreferredName = registration.PreferredName
            };
        }
    }
}