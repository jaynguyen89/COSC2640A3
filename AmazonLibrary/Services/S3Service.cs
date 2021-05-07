using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using AmazonLibrary.Contexts;
using AmazonLibrary.Interfaces;
using Helper.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AmazonLibrary.Services {

    public sealed class S3Service : IS3Service {

        private readonly ILogger<S3Service> _logger;
        private readonly IOptions<AmazonOptions> _options;
        private readonly IAmazonS3 _s3Context;

        public S3Service(
            ILogger<S3Service> logger,
            IOptions<AmazonOptions> options,
            AmazonS3Context s3Context
        ) {
            _logger = logger;
            _options = options;
            _s3Context = s3Context.GetInstance();
        }

        public async Task<string> UploadFileForImportToS3Bucket(Stream fileStream, SharedEnums.ImportType importType) {
            _logger.LogInformation($"{ nameof(S3Service) }.{ nameof(UploadFileForImportToS3Bucket) }: Service starts.");
            
            var bucketNameToSave = importType == SharedEnums.ImportType.Classroom
                ? _options.Value.S3BucketTeacherImportingClassrooms
                : _options.Value.S3BucketTeacherImportingStudentsToClassroom;

            var uploader = new TransferUtility(_s3Context);
            var fileId = Guid.NewGuid().ToString();

            try {
                await uploader.UploadAsync(fileStream, bucketNameToSave, fileId);
                await _s3Context.MakeObjectPublicAsync(bucketNameToSave, fileId, true);

                return fileId;
            }
            catch (Exception e) {
                _logger.LogError($"{ nameof(S3Service) }.{ nameof(UploadFileForImportToS3Bucket) }: { e.Message }\n\n{ e.StackTrace }");
                await uploader.AbortMultipartUploadsAsync(bucketNameToSave, DateTime.UtcNow);
                return default;
            }
        }

        public async Task<bool> CreateBucketWithNameIfNeeded(string bucketName) {
            _logger.LogInformation($"{ nameof(S3Service) }.{ nameof(CreateBucketWithNameIfNeeded) }: Service starts.");
            
            if (await _s3Context.DoesS3BucketExistAsync(bucketName))
                return true;
            
            var createBucketRequest = new PutBucketRequest {
                BucketName = bucketName,
                UseClientRegion = true,
                ObjectLockEnabledForBucket = false,
                CannedACL = S3CannedACL.NoACL
            };

            var response = await _s3Context.PutBucketAsync(createBucketRequest);
            return response.HttpStatusCode == HttpStatusCode.OK &&
                   !string.IsNullOrEmpty(response.ResponseMetadata.RequestId);
        }

        public async Task<string> UploadFileToBucket(string bucketName, Stream fileStream) {
            _logger.LogInformation($"{ nameof(S3Service) }.{ nameof(UploadFileToBucket) }: Service starts.");
            
            var uploader = new TransferUtility(_s3Context);
            
            try {
                var fileKey = Guid.NewGuid().ToString();
                await uploader.UploadAsync(fileStream, bucketName, fileKey);
                await _s3Context.MakeObjectPublicAsync(bucketName, fileKey, true);
                
                return fileKey;
            }
            catch (Exception e) {
                _logger.LogError($"{ nameof(S3Service) }.{ nameof(UploadFileToBucket) } - { nameof(Exception) }: { e.StackTrace }");

                await uploader.AbortMultipartUploadsAsync(bucketName, DateTime.UtcNow);
                return default;
            }
        }

        public async Task<bool> DeleteFileInS3Bucket(string bucketName, string fileId) {
            var deleteFileRequest = new DeleteObjectRequest {
                BucketName = bucketName,
                Key = fileId
            };

            var response = await _s3Context.DeleteObjectAsync(deleteFileRequest);
            return !string.IsNullOrEmpty(response.ResponseMetadata.RequestId);
        }
    }
}