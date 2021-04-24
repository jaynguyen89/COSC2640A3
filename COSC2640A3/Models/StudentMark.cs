using System;
using System.Collections.Generic;

#nullable disable

namespace COSC2640A3.Models
{
    public partial class StudentMark
    {
        public string Id { get; set; }
        public string EnrolmentId { get; set; }
        public string AssessmentId { get; set; }
        public byte Marks { get; set; }
        public DateTime MarkedOn { get; set; }
        public string Comment { get; set; }

        public virtual Assessment Assessment { get; set; }
        public virtual Enrolment Enrolment { get; set; }
    }
}
