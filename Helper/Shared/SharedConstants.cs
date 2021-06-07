using System.Collections.Generic;
using System.IO;

namespace Helper.Shared {

    public static class SharedConstants {

        public const string ClientUrl = @"http://ec2-3-25-62-10.ap-southeast-2.compute.amazonaws.com";

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
        
        public static readonly string EmailTemplateFolderPath =
            Path.GetDirectoryName(Directory.GetCurrentDirectory()) +
            $"/cosc2640a3/AmazonLibrary/Templates/";
        
        public static readonly List<string> InvalidTokens = new() {
            "--", "_@", "-@", ".-", "-.", "._", "_.", "@_", "@-", "__", "..", "_-", "-_"
        };
        
        public static readonly List<string> InvalidEnds = new() { ".", "-", "_" };
        public static readonly List<string> SpecialChars = new() { "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "-", "+", "=", "{", "[", "}", "]", ":", ";", "<", ",", ">", ".", "?", "|", "~" };

        public static readonly List<string> ExpectedClassroomAndStudentImportFileTypes = new() { "json", "csv" };
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

        public static readonly string[] EmailDomains = {
            "gmail.com", "yahoo.com", "hotmail.com", "msn.com", "live.com",
            "outlook.com", "ymail.com", "googlemail.com", "sky.com", "mail.com",
            "aim.com", "icloud.com", "apple.com", "microsoft.com", "qq.com",
            "verizon.net", "yandex.ru", "telstra.au", "skype.me", "edu.com"
        };
    }
}
