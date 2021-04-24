using System;
using System.Collections.Generic;

#nullable disable

namespace COSC2640A3.Models
{
    public partial class Account
    {
        public Account()
        {
            AccountRoles = new HashSet<AccountRole>();
            Students = new HashSet<Student>();
            Teachers = new HashSet<Teacher>();
        }

        public string Id { get; set; }
        public string EmailAddress { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Username { get; set; }
        public string NormalizedUsername { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string TwoFaSecretKey { get; set; }
        public string RecoveryToken { get; set; }
        public DateTime? TokenSetOn { get; set; }
        public string PreferredName { get; set; }

        public virtual ICollection<AccountRole> AccountRoles { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<Teacher> Teachers { get; set; }
    }
}
