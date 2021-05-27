using System.Collections.Generic;

namespace AssistantLibrary.Models {

    public sealed class Translation {
        
        public string Word { get; set; }
        
        public string RootForm { get; set; }
        
        public string[] WordTypes { get; set; }
        
        public Dictionary<string, string[]> Translations { get; set; }
        
        public Dictionary<string, string[]> Synonyms { get; set; }
        
        public Dictionary<string, string[]> Antonyms { get; set; }
    }
}