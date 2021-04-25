using System;
using System.Collections.Generic;

#nullable disable

namespace COSC2640A3.Models
{
    public partial class StudentMark
    {
        public string Id { get; set; }
        public string EnrolmentId { get; set; }
        public byte MarkBreakdowns { get; set; }
        public string Comment { get; set; }

        public virtual Enrolment Enrolment { get; set; }
    }
}
