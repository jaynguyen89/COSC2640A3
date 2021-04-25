using System.Collections.Generic;
using System.Linq;
using Helper.Shared;
using Microsoft.AspNetCore.Http;

namespace COSC2640A3.Bindings {

    public sealed class FileImportUpload {
        
        public SharedEnums.ImportType ImportType { get; set; }
        
        public IFormFile FileForImport { get; set; }

        public string[] VerifyUploading() {
            var errors = new List<string>();
            
            if (!SharedConstants.ExpectedClassroomAndStudentImportFileTypes.Any(FileForImport.ContentType.Contains))
                errors.Add("File type not supported. Expected JSON or CSV file.");
            
            if (FileForImport.Length == 0)
                errors.Add("File is empty. Please select another file.");

            return errors.ToArray();
        }
    }
}