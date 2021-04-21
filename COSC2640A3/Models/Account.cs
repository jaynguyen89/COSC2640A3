using System;
using System.Collections.Generic;

#nullable disable

namespace COSC2640A3.Models
{
    public partial class Account
    {
        public string Id { get; set; }
        public string EmailAddress { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Username { get; set; }
        public string NormalizedUsername { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string TwoFaSecretKey { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string RecoveryToken { get; set; }
        public DateTime? TokenSetOn { get; set; }
    }
}
