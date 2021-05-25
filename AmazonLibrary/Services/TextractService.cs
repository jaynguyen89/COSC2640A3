using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.Textract;
using Amazon.Textract.Model;
using AmazonLibrary.Contexts;
using AmazonLibrary.Interfaces;
using Helper.Shared;
using Microsoft.Extensions.Logging;

namespace AmazonLibrary.Services {

    public sealed class TextractService : ITextractService {
        
        private readonly ILogger<TextractService> _logger;
        private readonly IAmazonTextract _textractContext;

        public TextractService(
            ILogger<TextractService> logger,
            AmazonTextractContext textractContext
        ) {
            _logger = logger;
            _textractContext = textractContext.GetInstance();
        }

        public async Task<string> StartTextDetectionJobForFile(string fileId) {
            _logger.LogInformation($"{ nameof(TextractService) }.{ nameof(StartTextDetectionJobForFile) }: Service starts.");

            try {
                var startDetectionResponse = await _textractContext.StartDocumentTextDetectionAsync(
                    new StartDocumentTextDetectionRequest {
                        ClientRequestToken = Guid.NewGuid().ToString(),
                        DocumentLocation = new DocumentLocation {
                            S3Object = new S3Object {
                                Bucket = SharedConstants.TextractBucketName,
                                Name = fileId
                            }
                        }
                    }
                );

                return startDetectionResponse.JobId;
            }
            catch (AccessDeniedException e) {
                _logger.LogError($"{ nameof(TextractService) }.{ nameof(StartTextDetectionJobForFile) } - { nameof(AccessDeniedException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (BadDocumentException e) {
                _logger.LogWarning($"{ nameof(TextractService) }.{ nameof(StartTextDetectionJobForFile) } - { nameof(BadDocumentException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (DocumentTooLargeException e) {
                _logger.LogWarning($"{ nameof(TextractService) }.{ nameof(StartTextDetectionJobForFile) } - { nameof(DocumentTooLargeException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (IdempotentParameterMismatchException e) {
                _logger.LogWarning($"{ nameof(TextractService) }.{ nameof(StartTextDetectionJobForFile) } - { nameof(IdempotentParameterMismatchException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (InternalServerErrorException e) {
                _logger.LogError($"{ nameof(TextractService) }.{ nameof(StartTextDetectionJobForFile) } - { nameof(InternalServerErrorException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (InvalidKMSKeyException e) {
                _logger.LogError($"{ nameof(TextractService) }.{ nameof(StartTextDetectionJobForFile) } - { nameof(InvalidKMSKeyException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (InvalidParameterException e) {
                _logger.LogWarning($"{ nameof(TextractService) }.{ nameof(StartTextDetectionJobForFile) } - { nameof(InvalidParameterException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (InvalidS3ObjectException e) {
                _logger.LogWarning($"{ nameof(TextractService) }.{ nameof(StartTextDetectionJobForFile) } - { nameof(InvalidS3ObjectException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (LimitExceededException e) {
                _logger.LogError($"{ nameof(TextractService) }.{ nameof(StartTextDetectionJobForFile) } - { nameof(LimitExceededException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (ProvisionedThroughputExceededException e) {
                _logger.LogError($"{ nameof(TextractService) }.{ nameof(StartTextDetectionJobForFile) } - { nameof(ProvisionedThroughputExceededException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (ThrottlingException e) {
                _logger.LogError($"{ nameof(TextractService) }.{ nameof(StartTextDetectionJobForFile) } - { nameof(ThrottlingException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (UnsupportedDocumentException e) {
                _logger.LogWarning($"{ nameof(TextractService) }.{ nameof(StartTextDetectionJobForFile) } - { nameof(UnsupportedDocumentException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<string[]> GetExtractedTextsFromTextractJob(string jobId) {
            _logger.LogInformation($"{ nameof(TextractService) }.{ nameof(StartTextDetectionJobForFile) }: Service starts.");

            try {
                var detectionRequest = new GetDocumentTextDetectionRequest { JobId = jobId };
                var detectionResponse = new GetDocumentTextDetectionResponse { JobStatus = JobStatus.IN_PROGRESS };

                while (detectionResponse.JobStatus == JobStatus.IN_PROGRESS) {
                    detectionResponse = await _textractContext.GetDocumentTextDetectionAsync(detectionRequest);
                    await Task.Delay(TimeSpan.FromMilliseconds(SharedConstants.DefaultTaskWaiting * 6));
                }

                if (detectionResponse.JobStatus != JobStatus.SUCCEEDED) return null;

                var extractedTexts = new List<string>();
                while (detectionResponse.NextToken is not null) {
                    extractedTexts.AddRange(detectionResponse.Blocks.Select(block => block.Text));

                    detectionRequest.NextToken = detectionResponse.NextToken;
                    detectionResponse = await _textractContext.GetDocumentTextDetectionAsync(detectionRequest);
                }

                return extractedTexts.ToArray();
            }
            catch (AccessDeniedException e) {
                _logger.LogError($"{ nameof(TextractService) }.{ nameof(GetExtractedTextsFromTextractJob) } - { nameof(AccessDeniedException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
            catch (InternalServerErrorException e) {
                _logger.LogError($"{ nameof(TextractService) }.{ nameof(GetExtractedTextsFromTextractJob) } - { nameof(InternalServerErrorException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
            catch (InvalidJobIdException e) {
                _logger.LogWarning($"{ nameof(TextractService) }.{ nameof(GetExtractedTextsFromTextractJob) } - { nameof(InvalidJobIdException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
            catch (InvalidParameterException e) {
                _logger.LogWarning($"{ nameof(TextractService) }.{ nameof(GetExtractedTextsFromTextractJob) } - { nameof(InvalidParameterException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
            catch (InvalidS3ObjectException e) {
                _logger.LogWarning($"{ nameof(TextractService) }.{ nameof(GetExtractedTextsFromTextractJob) } - { nameof(InvalidS3ObjectException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
            catch (ProvisionedThroughputExceededException e) {
                _logger.LogError($"{ nameof(TextractService) }.{ nameof(GetExtractedTextsFromTextractJob) } - { nameof(ProvisionedThroughputExceededException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
            catch (ThrottlingException e) {
                _logger.LogError($"{ nameof(TextractService) }.{ nameof(GetExtractedTextsFromTextractJob) } - { nameof(ThrottlingException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
        }

        public async Task<string[]> DetectSimpleDocumentTexts(string fileId) {
            _logger.LogInformation($"{ nameof(TextractService) }.{ nameof(DetectSimpleDocumentTexts) }: Service starts.");

            var request = new DetectDocumentTextRequest {
                Document = new Document {
                    S3Object = new S3Object {
                        Name = fileId,
                        Bucket = SharedConstants.TextractBucketName
                    }
                }
            };

            try {
                var result = await _textractContext.DetectDocumentTextAsync(request);
                if (result.HttpStatusCode != HttpStatusCode.OK) throw new InternalServerErrorException("Request to AWS Textract failed.");

                return result.Blocks.Where(block => block.BlockType != BlockType.WORD).Select(block => block.Text).ToArray();
            }
            catch (AccessDeniedException e) {
                _logger.LogError($"{ nameof(TextractService) }.{ nameof(GetExtractedTextsFromTextractJob) } - { nameof(AccessDeniedException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
            catch (BadDocumentException e) {
                _logger.LogError($"{ nameof(TextractService) }.{ nameof(GetExtractedTextsFromTextractJob) } - { nameof(BadDocumentException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
            catch (DocumentTooLargeException e) {
                _logger.LogError($"{ nameof(TextractService) }.{ nameof(GetExtractedTextsFromTextractJob) } - { nameof(DocumentTooLargeException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
            catch (InternalServerErrorException e) {
                _logger.LogError($"{ nameof(TextractService) }.{ nameof(GetExtractedTextsFromTextractJob) } - { nameof(InternalServerErrorException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
            catch (InvalidParameterException e) {
                _logger.LogError($"{ nameof(TextractService) }.{ nameof(GetExtractedTextsFromTextractJob) } - { nameof(InvalidParameterException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
            catch (InvalidS3ObjectException e) {
                _logger.LogError($"{ nameof(TextractService) }.{ nameof(GetExtractedTextsFromTextractJob) } - { nameof(InvalidS3ObjectException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
            catch (ProvisionedThroughputExceededException e) {
                _logger.LogError($"{ nameof(TextractService) }.{ nameof(GetExtractedTextsFromTextractJob) } - { nameof(ProvisionedThroughputExceededException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
            catch (ThrottlingException e) {
                _logger.LogError($"{ nameof(TextractService) }.{ nameof(GetExtractedTextsFromTextractJob) } - { nameof(ThrottlingException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
            catch (UnsupportedDocumentException e) {
                _logger.LogError($"{ nameof(TextractService) }.{ nameof(GetExtractedTextsFromTextractJob) } - { nameof(UnsupportedDocumentException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
        }
    }
}