using System;
using System.Collections.Generic;

#nullable disable

namespace COSC2640A3.Models
{
    public partial class Assessment
    {
        public Assessment()
        {
            StudentMarks = new HashSet<StudentMark>();
        }

        public string Id { get; set; }
        public string ClassroomId { get; set; }
        public string AssessmentName { get; set; }
        public string Description { get; set; }
        public byte TotalMark { get; set; }
        public DateTime ReleasedOn { get; set; }

        public virtual Classroom Classroom { get; set; }
        public virtual ICollection<StudentMark> StudentMarks { get; set; }
    }
}
