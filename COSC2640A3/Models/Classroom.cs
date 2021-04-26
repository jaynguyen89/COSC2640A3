using System;
using System.Collections.Generic;

#nullable disable

namespace COSC2640A3.Models
{
    public partial class Classroom
    {
        public Classroom()
        {
            Enrolments = new HashSet<Enrolment>();
        }

        public string Id { get; set; }
        public string TeacherId { get; set; }
        public string ClassName { get; set; }
        public short Capacity { get; set; }
        public decimal Price { get; set; }
        public DateTime? CommencedOn { get; set; }
        public byte Duration { get; set; }
        public byte DurationUnit { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual Teacher Teacher { get; set; }
        public virtual ICollection<Enrolment> Enrolments { get; set; }
    }
}
