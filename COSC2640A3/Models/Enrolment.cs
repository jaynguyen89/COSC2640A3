using System;
using System.Collections.Generic;

#nullable disable

namespace COSC2640A3.Models
{
    public partial class Enrolment
    {
        public Enrolment()
        {
            StudentMarks = new HashSet<StudentMark>();
        }

        public string Id { get; set; }
        public string StudentId { get; set; }
        public string ClassroomId { get; set; }
        public DateTime EnrolledOn { get; set; }
        public byte OverallMark { get; set; }
        public bool IsPassed { get; set; }

        public virtual Student Student { get; set; }
        public virtual ICollection<StudentMark> StudentMarks { get; set; }
    }
}
