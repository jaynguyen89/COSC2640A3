using System.Collections.Generic;
using System.IO;

namespace Helper.Shared {

    public static class SharedConstants {

        public const string ProjectName = "COSC2640A2API";

        public const string MonoSpace = " ";
        public const string MultiSpace = @"\s+";
        public const string FSlash = "/";
        public const string BSlash = "\\";
        
        public static readonly string AssistantLibraryTemplateFolderPath = Path.GetDirectoryName(Directory.GetCurrentDirectory()) + $"/AssistantLibrary/Templates/";
        
        public static readonly List<string> InvalidTokens = new() {
            "--", "_@", "-@", ".-", "-.", "._", "_.", "@_", "@-", "__", "..", "_-", "-_"
        };
        
        public static readonly List<string> InvalidEnds = new() { ".", "-", "_" };
        
        public static readonly List<string> SpecialChars = new() { "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "-", "+", "=", "{", "[", "}", "]", ":", ";", "<", ",", ">", ".", "?", "|", "~" };

        public static readonly List<string> ExpectedClassroomAndStudentImportFileTypes = new() { "json", "csv" };
    }
}
