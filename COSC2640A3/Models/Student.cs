using System;
using System.Collections.Generic;

#nullable disable

namespace COSC2640A3.Models
{
    public partial class Student
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string SchoolName { get; set; }
        public string Faculty { get; set; }
        public string PersonalUrl { get; set; }

        public virtual Account Account { get; set; }
    }
}
