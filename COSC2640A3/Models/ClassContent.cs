using System;
using System.Collections.Generic;

#nullable disable

namespace COSC2640A3.Models
{
    public partial class ClassContent
    {
        public string Id { get; set; }
        public string ClassroomId { get; set; }
        public string Videos { get; set; }
        public string Audios { get; set; }
        public string Photos { get; set; }
        public string Attachments { get; set; }
        public string HtmlContent { get; set; }

        public virtual Classroom Classroom { get; set; }
    }
}
