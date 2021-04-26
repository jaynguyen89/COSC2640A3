using System;
using System.Collections.Generic;

#nullable disable

namespace COSC2640A3.Models
{
    public partial class Enrolment
    {
        public string Id { get; set; }
        public string StudentId { get; set; }
        public string ClassroomId { get; set; }
        public string InvoiceId { get; set; }
        public DateTime EnrolledOn { get; set; }
        public byte? OverallMark { get; set; }
        public string MarkBreakdowns { get; set; }
        public bool? IsPassed { get; set; }

        public virtual Classroom Classroom { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual Student Student { get; set; }
    }
}
