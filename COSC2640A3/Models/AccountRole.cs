using System;
using System.Collections.Generic;

#nullable disable

namespace COSC2640A3.Models
{
    public partial class AccountRole
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public byte Role { get; set; }

        public virtual Account Account { get; set; }
    }
}
