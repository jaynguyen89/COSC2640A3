using System;
using System.Collections.Generic;

#nullable disable

namespace COSC2640A3.Models
{
    public partial class Teacher
    {
        public Teacher()
        {
            Classrooms = new HashSet<Classroom>();
        }

        public string Id { get; set; }
        public string AccountId { get; set; }
        public string Company { get; set; }
        public string JobTitle { get; set; }
        public string PersonalWebsite { get; set; }

        public virtual Account Account { get; set; }
        public virtual ICollection<Classroom> Classrooms { get; set; }
    }
}
