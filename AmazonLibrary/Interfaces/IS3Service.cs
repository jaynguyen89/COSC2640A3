using System.IO;
using System.Threading.Tasks;

namespace AmazonLibrary.Interfaces {

    public interface IS3Service {
        
        Task<string> UploadFileForImportToS3Bucket(Stream fileStream, string importType);
    }
}