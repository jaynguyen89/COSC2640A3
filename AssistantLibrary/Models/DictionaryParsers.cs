using Newtonsoft.Json;

namespace AssistantLibrary.Models {

    public sealed class Category {
        
        public string Id { get; set; }
        
        public string Text { get; set; }
    }

    public class EntryInfo {
        
        public string Id { get; set; }
            
        public string Language { get; set; }
            
        public string Word { get; set; }
    }
    
    public class LexiconInfo {
                            
        public string Language { get; set; }
                            
        public string Text { get; set; }
    }

    public class Lexicon : LexiconInfo {
                
        [JsonProperty("lexicalCategory")]
        public Category WordType { get; set; }
    }

    public sealed class LemmaParser {
        
        [JsonProperty("results")]
        public LemmaEntry[] LemmaEntries { get; set; }
        
        public sealed class LemmaEntry : EntryInfo {
            
            public LexicalEntry[] LexicalEntries { get; set; }
            
            public sealed class LexicalEntry : Lexicon {
                
                [JsonProperty("inflectionOf")]
                public Category[] Inflection { get; set; }
            }
        }
    }

    public sealed class ThesaurusParser {
        
        [JsonProperty("results")]
        public ThesaurusEntry[] ThesaurusEntries { get; set; }
        
        public sealed class ThesaurusEntry : EntryInfo {
            
            public string Type { get; set; }
            
            public LexicalEntry[] LexicalEntries { get; set; }
            
            public sealed class LexicalEntry : Lexicon {
                
                public Entry[] Entries { get; set; }
                
                public sealed class Entry {
                    
                    public SenseEntry[] Senses { get; set; }
                    
                    public sealed class SenseEntry {
                        
                        public string Id { get; set; }
                        
                        public LexiconInfo[] Synonyms { get; set; }
                        
                        public LexiconInfo[] Antonyms { get; set; }
                    }
                }
            }
        }
    }

    public sealed class TranslationParser {
        
        [JsonProperty("results")]
        public TranslationEntry[] TranslationEntries { get; set; }
        
        public sealed class TranslationEntry : EntryInfo {
            
            public string Type { get; set; }
            
            public LexicalEntry[] LexicalEntries { get; set; }
            
            public sealed class LexicalEntry : Lexicon {
                
                public Entry[] Entries { get; set; }
                
                public sealed class Entry {
                    
                    public Pronounciation[] Pronounciations { get; set; }
                    
                    public SenseEntry[] Senses { get; set; }
                    
                    public sealed class SenseEntry {
                        
                        public string Id { get; set; }
                        
                        public Translation[] Translations { get; set; }
                        
                        public sealed class Translation {
                            
                            public string Language { get; set; }
                            
                            public string Text { get; set; }
                            
                            [JsonProperty("grammaticalFeatures")]
                            public GrammarFeature[] GrammarFeatures { get; set; }

                            public sealed class GrammarFeature {
                                
                                public string Id { get; set; }
                                
                                public string Text { get; set; }
                                
                                public string Type { get; set; }
                            }
                        }
                    }
                    
                    public sealed class Pronounciation {
                                
                        [JsonProperty("phoneticNotation")]
                        public string Notation { get; set; }
                                
                        [JsonProperty("phoneticSpelling")]
                        public string Spelling { get; set; }
                    }
                }
            }
        }
    }
}