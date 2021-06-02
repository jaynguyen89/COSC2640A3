using System;
using System.Collections.Generic;
using System.Linq;
using Helper;
using Helper.Shared;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace COSC2640A3.Bindings {

    public class  ContentBinding {
        
        public string ClassroomId { get; set; }
        
        public byte FileType { get; set; }
    }

    public class FilesAdding : ContentBinding {
        
        public IFormFileCollection UploadedFiles { get; set; }

        public string[] VerifyAddedFiles() {
            var errors = new List<string>();
            
            if (FileType is < 0 or > (byte) SharedEnums.FileType.other) errors.Add($"{ nameof(FileType).ToHumanStyled() } is not recognized.");
            if (UploadedFiles is null || UploadedFiles.Count == 0) errors.Add($"The { nameof(UploadedFiles).ToHumanStyled() } is empty.");

            if (UploadedFiles is null) return errors.ToArray();
            foreach (var uploadedFile in UploadedFiles)
                switch (FileType) {
                    case (byte) SharedEnums.FileType.video:
                        if (!uploadedFile.ContentType.Contains(nameof(SharedEnums.FileType.video)))
                            errors.Add($"File { uploadedFile.FileName } is not a { nameof(SharedEnums.FileType.video) } file.");
                        break;
                    case (byte) SharedEnums.FileType.audio:
                        if (!uploadedFile.ContentType.Contains(nameof(SharedEnums.FileType.audio)))
                            errors.Add($"File { uploadedFile.FileName } is not an { nameof(SharedEnums.FileType.audio) } file.");
                        break;
                    case (byte) SharedEnums.FileType.image:
                        if (!uploadedFile.ContentType.Contains(nameof(SharedEnums.FileType.image)))
                            errors.Add($"File { uploadedFile.FileName } is not a { nameof(SharedEnums.FileType.image) } file.");
                        break;
                }

            return errors.ToArray();
        }
    }

    public sealed class FilesUpdating : FilesAdding {
        
        public string[] RemovedFiles { get; set; }

        public string[] VerifyUpdatedFiles() {
            var errors = new List<string>();

            RemovedFiles = RemovedFiles
                           .Select(JsonConvert.DeserializeObject<string[]>)
                           .SelectMany(z => z)
                           .ToArray();
            
            if (FileType is < 0 or > (byte) SharedEnums.FileType.other) errors.Add($"{ nameof(FileType).ToHumanStyled() } is not recognized.");
            if (UploadedFiles is null || UploadedFiles.Count == 0) return errors.ToArray();
            
            foreach (var uploadedFile in UploadedFiles)
                switch (FileType) {
                    case (byte) SharedEnums.FileType.video:
                        if (!uploadedFile.ContentType.Contains(nameof(SharedEnums.FileType.video)))
                            errors.Add($"File { uploadedFile.FileName } is not a { nameof(SharedEnums.FileType.video) } file.");
                        break;
                    case (byte) SharedEnums.FileType.audio:
                        if (!uploadedFile.ContentType.Contains(nameof(SharedEnums.FileType.audio)))
                            errors.Add($"File { uploadedFile.FileName } is not an { nameof(SharedEnums.FileType.audio) } file.");
                        break;
                    case (byte) SharedEnums.FileType.image:
                        if (!uploadedFile.ContentType.Contains(nameof(SharedEnums.FileType.image)))
                            errors.Add($"File { uploadedFile.FileName } is not a { nameof(SharedEnums.FileType.image) } file.");
                        break;
                }

            return errors.ToArray();
        }
    }
    
    public sealed class RichContent {
        
        public string ClassroomId { get; set; }
        
        public string HtmlContent { get; set; }

        public string[] VerifyRichContent() {
            return !Helpers.IsProperString(HtmlContent) ? new[] { "No content to be added." } : Array.Empty<string>();
        }
    }

    public sealed class RichContentImport {
        
        public string ClassroomId { get; set; }

        public IFormFileCollection FilesForImport { get; set; }
        
        public string[] VerifyRichContent() {
            var errors = new List<string>();
            
            if (FilesForImport.Count == 0) errors.Add($"The { nameof(FilesForImport).ToHumanStyled() } is empty.");
            errors.AddRange(
                from uploadedFile in FilesForImport
                where !SharedConstants.ExpectedTextractFileTypes
                                      .Any(uploadedFile.ContentType.Contains)
                select $"File {uploadedFile.FileName} is not a {nameof(SharedEnums.FileType.image)}."
            );

            return errors.ToArray();
        }
    }
    
    public sealed class DataExport {
        
        public string[] ClassroomIds { get; set; }
    }
}