using System;
using Newtonsoft.Json;

namespace COSC2640A3.ViewModels.ClassContent {

    public sealed class ClassContentVM {
        
        public string Id { get; set; }
        
        public FileVM[] Videos { get; set; }
        
        public FileVM[] Audios { get; set; }
        
        public FileVM[] Photos { get; set; }
        
        public FileVM[] Attachments { get; set; }
        
        public string HtmlContent { get; set; }

        public static implicit operator ClassContentVM(Models.ClassContent classContent) {
            return new() {
                Id = classContent.Id,
                Videos = classContent.Videos is null ? Array.Empty<FileVM>() : JsonConvert.DeserializeObject<FileVM[]>(classContent.Videos),
                Audios = classContent.Audios is null ? Array.Empty<FileVM>() : JsonConvert.DeserializeObject<FileVM[]>(classContent.Audios),
                Photos = classContent.Photos is null ? Array.Empty<FileVM>() : JsonConvert.DeserializeObject<FileVM[]>(classContent.Photos),
                Attachments = classContent.Attachments is null ? Array.Empty<FileVM>() : JsonConvert.DeserializeObject<FileVM[]>(classContent.Attachments),
                HtmlContent = classContent.HtmlContent
            };
        }
    }
}