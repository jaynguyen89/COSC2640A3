using System.IO;

namespace Helper.Shared {

    public static class SharedConstants {

        public const string ProjectName = "COSC2640A2API";

        public const string FSlash = "/";
        public const string BSlash = "\\";
        
        public static readonly string AssistantLibraryTemplateFolderPath = Path.GetDirectoryName(Directory.GetCurrentDirectory()) + $"/AssistantLibrary/Templates/";
    }
}
