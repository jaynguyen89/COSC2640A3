using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Helper.Shared;
using Helper;

namespace COSC2640A3.Bindings {

    public class TranslateIt {
        
        public SharedEnums.Language TargetLanguage { get; set; }
        
        public string Phrase { get; set; }
        
        public bool ForSynonyms { get; set; }

        public string[] VerifyTranslation() {
            var errors = new List<string>();
            
            if (TargetLanguage == SharedEnums.Language.English) errors.Add($"{ nameof(TargetLanguage).ToHumanStyled() } should not be English. No translation required.");
            if (!Helpers.IsProperString(Phrase)) errors.Add($"{ nameof(Phrase) } is missing.");

            Phrase = Regex.Replace(Phrase.Trim(), SharedConstants.MultiSpace, SharedConstants.MonoSpace);
            if (Encoding.Unicode.GetByteCount(Phrase) > 4500) errors.Add($"The { nameof(Phrase) } is too long for translation. ");

            return errors.ToArray();
        }
    }
}