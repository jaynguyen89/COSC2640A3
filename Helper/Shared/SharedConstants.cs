using System.Collections.Generic;
using System.IO;

namespace Helper.Shared {

    public static class SharedConstants {

        public const string ProjectName = "COSC2640A2API";
        public const string TwoFaCacheName = "IsTwoFaConfirmed";
        public const string TextractBucketName = "class.content.textract.imports";

        public const int DefaultTaskWaiting = 1500; //ms

        public const string MonoSpace = " ";
        public const string MultiSpace = @"\s+";
        public const string FSlash = "/";
        public const string BSlash = "\\";
        public const string DivOpen = "<div>";
        public const string DivClose = "</div>";
        public const string ParaOpen = "<p>";
        public const string ParaClose = "<p/>";
        
        public static readonly string EmailTemplateFolderPath = Path.GetDirectoryName(Directory.GetCurrentDirectory()) + $"\\AmazonLibrary\\Templates\\";
        
        public static readonly List<string> InvalidTokens = new() {
            "--", "_@", "-@", ".-", "-.", "._", "_.", "@_", "@-", "__", "..", "_-", "-_"
        };
        
        public static readonly List<string> InvalidEnds = new() { ".", "-", "_" };
        public static readonly List<string> SpecialChars = new() { "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "-", "+", "=", "{", "[", "}", "]", ":", ";", "<", ",", ">", ".", "?", "|", "~" };

        public static readonly List<string> ExpectedClassroomAndStudentImportFileTypes = new() { "json", "csv" };
        public static readonly List<string> ExpectedClassContentVideoTypes = new() { "mp4", "mov", "wmv", "flv", "avi", "mkv" };
        public static readonly List<string> ExpectedClassContentAudioTypes = new() { "mp3", "wav", "wma", "aac", "flac" };
        public static readonly List<string> ExpectedClassContentPhotoTypes = new() { "jpg", "jpeg", "png", "bmp", "tiff", "gif" };
        public static readonly List<string> ExpectedTextractFileTypes = new() { "jpg", "jpeg", "png", "pdf" };

        public static readonly Dictionary<byte, string> LanguageCodes = new() {
            { (byte) SharedEnums.Language.English, "en" },
            { (byte) SharedEnums.Language.Chinese, "zh" },
            { (byte) SharedEnums.Language.French, "fr" },
            { (byte) SharedEnums.Language.German, "de" },
            { (byte) SharedEnums.Language.Greek, "el" },
            { (byte) SharedEnums.Language.Hindi, "hi" },
            { (byte) SharedEnums.Language.Indonesian, "id" },
            { (byte) SharedEnums.Language.Italian, "it" },
            { (byte) SharedEnums.Language.Malay, "ms" },
            { (byte) SharedEnums.Language.Portuguese, "pt" },
            { (byte) SharedEnums.Language.Russian, "ru" },
            { (byte) SharedEnums.Language.Spanish, "es" }
        };
    }
}
