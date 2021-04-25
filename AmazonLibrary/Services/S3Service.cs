using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Transfer;
using AmazonLibrary.Contexts;
using AmazonLibrary.Interfaces;
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

        public async Task<string> UploadFileForImportToS3Bucket(Stream fileStream, string importType) {
            _logger.LogInformation($"{ nameof(S3Service) }.{ nameof(UploadFileForImportToS3Bucket) }: Service starts.");
            
            var bucketNameToSave = importType.Equals("Classroom")
                ? _options.Value.S3BucketTeacherImportingClassrooms
                : (importType.Equals("Students")
                    ? _options.Value.S3BucketTeacherImportingStudentsToClassroom
                    : _options.Value.S3BucketTeacherImportingClassroomsAndStudents
                );

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
    }
}