using System.IO;
using System.Threading.Tasks;
using Helper.Shared;

namespace AmazonLibrary.Interfaces {

    public interface IS3Service {
        
        Task<string> UploadFileForImportToS3Bucket(Stream fileStream, SharedEnums.ImportType importType);
        
        Task<bool> CreateBucketWithNameIfNeeded(string bucketName);
        
        /// <summary>
        /// Returns the ID of file in S3 bucket.
        /// </summary>
        Task<string> UploadFileToBucket(string bucketName, Stream fileStream);
        
        Task<bool> DeleteFileInS3Bucket(string bucketName, string fileId);
    }
}