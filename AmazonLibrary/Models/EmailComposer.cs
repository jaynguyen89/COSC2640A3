using System.Collections.Generic;
using static Helper.Shared.SharedEnums;

namespace AmazonLibrary.Models {

    public sealed class EmailComposer {
        
        public EmailType EmailType { get; set; }
        
        public string ReceiverEmail { get; set; }
        
        public string Subject { get; set; }
        
        public Dictionary<string, string> Contents { get; set; }
    }
}