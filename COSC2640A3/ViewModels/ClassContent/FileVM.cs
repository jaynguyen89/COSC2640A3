namespace COSC2640A3.ViewModels.ClassContent {

    public sealed class FileVM {
        
        public string Id { get; set; }
        
        public string Name { get; set; }
        
        public byte Type { get; set; }
        
        public string Extension { get; set; }
        
        public long UploadedOn { get; set; }
    }
}