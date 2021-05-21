﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AmazonLibrary.Interfaces;
using COSC2640A3.Attributes;
using COSC2640A3.Bindings;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.ViewModels;
using COSC2640A3.ViewModels.ClassContent;
using Helper;
using Helper.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static Helper.Shared.SharedEnums;

namespace COSC2640A3.Controllers {

    [ApiController]
    [MainAuthorize]
    [TwoFaAuthorize]
    [Route("class-content")]
    public sealed class ClassContentController {

        private readonly ILogger<ClassContentController> _logger;
        private readonly IClassContentService _classContentService;
        private readonly IClassroomService _classroomService;
        private readonly IS3Service _s3Service;
        private readonly ITextractService _textractService;

        public ClassContentController(
            ILogger<ClassContentController> logger,
            IClassContentService classContentService,
            IClassroomService classroomService,
            IS3Service s3Service,
            ITextractService textractService
        ) {
            _logger = logger;
            _classContentService = classContentService;
            _classroomService = classroomService;
            _s3Service = s3Service;
            _textractService = textractService;
        }

        /// <summary>
        /// For teacher. To upload files for classroom contents. Supports videos, audios, images and any other types as specified in request body by Enum value.
        /// Each request sent to this endpoint is to add files of 1 specified type.
        /// Files will be uploaded to S3 and corresponding data will be saved in database.
        /// </summary>
        /// <remarks>
        /// Request signature:
        ///     POST /class-content/add-files
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        ///         "Content-Type": "multipart/form-data"
        ///     Body
        ///         {
        ///             classroomId: string,
        ///             fileType: 0 | 1 | 2 | 3,
        ///             uploadedFiles: [binary]
        ///         }
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="filesToAdd">The data containing files binary to be added.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [HttpPost("add-files")]
        [RoleAuthorize(Role.Teacher)]
        public async Task<JsonResult> AddFilesToClassroom([FromHeader] string accountId,[FromForm] FilesAdding filesToAdd) {
            _logger.LogInformation($"{ nameof(ClassContentController) }.{ nameof(AddFilesToClassroom) }: Service starts.");

            var errors = filesToAdd.VerifyAddedFiles();
            if (errors.Length != 0) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Data = errors });
            
            var isBelonged = await _classroomService.IsClassroomBelongedToThisTeacherByAccountId(accountId, filesToAdd.ClassroomId);
            if (!isBelonged.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isBelonged.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });
            
            var classContent = await _classContentService.GetClassContentByClassroomId(filesToAdd.ClassroomId);
            classContent ??= new ClassContent { ClassroomId = filesToAdd.ClassroomId };

            var addFilesExpression = filesToAdd.FileType switch {
                (byte) FileType.video => (Func<Task<FileVM[]>>)(async () => await UploadVideosToS3Bucket(filesToAdd.ClassroomId, filesToAdd.UploadedFiles)),
                (byte) FileType.audio => async () => await UploadAudiosToS3Bucket(filesToAdd.ClassroomId, filesToAdd.UploadedFiles),
                (byte) FileType.photo => async () => await UploadPhotosToS3Bucket(filesToAdd.ClassroomId, filesToAdd.UploadedFiles),
                (byte) FileType.other => async () => await UploadAttachmentsToS3Bucket(filesToAdd.ClassroomId, filesToAdd.UploadedFiles),
                _ => default
            };

            var uploadResults = addFilesExpression?.Invoke().Result;
            if (uploadResults is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            
            _ = filesToAdd.FileType switch {
                (byte) FileType.video => classContent.Videos = JsonConvert.SerializeObject(uploadResults),
                (byte) FileType.audio => classContent.Audios = JsonConvert.SerializeObject(uploadResults),
                (byte) FileType.photo => classContent.Photos = JsonConvert.SerializeObject(uploadResults),
                (byte) FileType.other => classContent.Attachments = JsonConvert.SerializeObject(uploadResults),
                _ => default
            };

            var saveDatabaseResult = !Helpers.IsProperString(classContent.Id)
                ? await _classContentService.InsertNewContent(classContent) is not null
                : await _classContentService.UpdateContent(classContent);
            
            return !saveDatabaseResult.HasValue || !saveDatabaseResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }
        
        /// <summary>
        /// For teacher. To upload files for classroom contents. Supports videos, audios, images and any other types as specified in request body by Enum value.
        /// Each request sent to this endpoint is to add files of 1 specified type. Request body can specify files to be removed, and files to be added more.
        /// One or both of the 2 optional property in request body must have value, they can't be null at the same time.
        /// </summary>
        /// <remarks>
        /// Request signature:
        ///     POST /class-content/update-files
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        ///         "Content-Type": "multipart/form-data"
        ///     Body
        ///         {
        ///             classroomId: string,
        ///             fileType: 0 | 1 | 2 | 3,
        ///             uploadedFiles: [binary], ---> optional: select files to add if any
        ///             removedFiles: [string] ---> optional: select files to remove if any
        ///         }
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="filesToUpdate">The data containing files binary and serialized data to be updated.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [HttpPost("update-files")]
        [RoleAuthorize(Role.Teacher)]
        public async Task<JsonResult> UpdateFilesForClassroom([FromHeader] string accountId,[FromForm] FilesUpdating filesToUpdate) {
            _logger.LogInformation($"{ nameof(ClassContentController) }.{ nameof(UpdateFilesForClassroom) }: Service starts.");

            var errors = filesToUpdate.VerifyUpdatedFiles();
            if (errors.Length != 0) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Data = errors });
            
            var isBelonged = await _classroomService.IsClassroomBelongedToThisTeacherByAccountId(accountId, filesToUpdate.ClassroomId);
            if (!isBelonged.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isBelonged.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });

            var classContent = await _classContentService.GetClassContentByClassroomId(filesToUpdate.ClassroomId);
            if (classContent is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var contentFiles = filesToUpdate.FileType switch {
                (byte) FileType.video => JsonConvert.DeserializeObject<FileVM[]>(classContent.Videos),
                (byte) FileType.audio => JsonConvert.DeserializeObject<FileVM[]>(classContent.Audios),
                (byte) FileType.photo => JsonConvert.DeserializeObject<FileVM[]>(classContent.Photos),
                (byte) FileType.other => JsonConvert.DeserializeObject<FileVM[]>(classContent.Attachments),
                _ => default
            };

            if (contentFiles is not null && filesToUpdate.RemovedFiles is not null && filesToUpdate.RemovedFiles.Length != 0) {
                var fileIdsToDelete = contentFiles.Where(file => filesToUpdate.RemovedFiles.Contains(file.Id)).Select(file => file.Id).ToArray();
                
                var bucketName = GetBucketNameForFileType(filesToUpdate.ClassroomId, filesToUpdate.FileType);
                _ = fileIdsToDelete.Select(async fileId => await _s3Service.DeleteFileInS3Bucket(bucketName, fileId));
                
                contentFiles = contentFiles.Where(file => !filesToUpdate.RemovedFiles.Contains(file.Id)).ToArray();
            }

            if (filesToUpdate.UploadedFiles is not null && filesToUpdate.UploadedFiles.Count != 0) {
                contentFiles ??= Array.Empty<FileVM>();
                
                var addFilesExpression = filesToUpdate.FileType switch {
                    (byte) FileType.video => (Func<Task<FileVM[]>>)(async () => await UploadVideosToS3Bucket(filesToUpdate.ClassroomId, filesToUpdate.UploadedFiles)),
                    (byte) FileType.audio => async () => await UploadAudiosToS3Bucket(filesToUpdate.ClassroomId, filesToUpdate.UploadedFiles),
                    (byte) FileType.photo => async () => await UploadPhotosToS3Bucket(filesToUpdate.ClassroomId, filesToUpdate.UploadedFiles),
                    (byte) FileType.other => async () => await UploadAttachmentsToS3Bucket(filesToUpdate.ClassroomId, filesToUpdate.UploadedFiles),
                    _ => default
                };

                var uploadResults = addFilesExpression?.Invoke().Result;
                if (uploadResults is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
                
                contentFiles = uploadResults.Aggregate(contentFiles, (current, uploadResult) => current.Append(uploadResult).ToArray());
            }

            var hasContent = contentFiles is not null && contentFiles.Length != 0;
            _ = filesToUpdate.FileType switch {
                (byte) FileType.video => classContent.Videos = hasContent ? JsonConvert.SerializeObject(contentFiles) : default,
                (byte) FileType.audio => classContent.Audios = hasContent ? JsonConvert.SerializeObject(contentFiles) : default,
                (byte) FileType.photo => classContent.Photos = hasContent ? JsonConvert.SerializeObject(contentFiles) : default,
                (byte) FileType.other => classContent.Attachments = hasContent ? JsonConvert.SerializeObject(contentFiles) : default,
                _ => default
            };

            var updateDatabaseResult = await _classContentService.UpdateContent(classContent);
            return !updateDatabaseResult.HasValue || !updateDatabaseResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }
        
        /// <summary>
        /// For teacher. To add rich-text content to classroom contents.
        /// </summary>
        /// <remarks>
        /// Request signature:
        ///     POST /class-content/add-rich-content
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        ///     Body
        ///         {
        ///             classroomId: string,
        ///             htmlContent: string
        ///         }
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="richContent">The required data to be added.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [HttpPost("add-rich-content")]
        [RoleAuthorize(Role.Teacher)]
        public async Task<JsonResult> AddContentToClassroom([FromHeader] string accountId,[FromBody] RichContent richContent) {
            _logger.LogInformation($"{ nameof(ClassContentController) }.{ nameof(AddContentToClassroom) }: Service starts.");

            var errors = richContent.VerifyRichContent();
            if (errors.Length != 0) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Data = errors });
            
            var isBelonged = await _classroomService.IsClassroomBelongedToThisTeacherByAccountId(accountId, richContent.ClassroomId);
            if (!isBelonged.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isBelonged.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });
            
            var classContent = await _classContentService.GetClassContentByClassroomId(richContent.ClassroomId);
            classContent ??= new ClassContent { ClassroomId = richContent.ClassroomId };

            classContent.HtmlContent = richContent.HtmlContent;
            
            var saveDatabaseResult = !Helpers.IsProperString(classContent.Id)
                ? await _classContentService.InsertNewContent(classContent) is not null
                : await _classContentService.UpdateContent(classContent);
            
            return !saveDatabaseResult.HasValue || !saveDatabaseResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }
        
        /// <summary>
        /// For teacher. To add rich-text content to classroom contents by uploaded images containing texts to be extracted programmatically.
        /// </summary>
        /// <remarks>
        /// Request signature:
        ///     POST /class-content/import-rich-content
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        ///         "Content-Type": "multipart/form-data"
        ///     Body
        ///         {
        ///             classroomId: string,
        ///             filesForImport: [binary]
        ///         }
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="richContent">The required data to be added.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [HttpPost("import-rich-content")]
        [RoleAuthorize(Role.Teacher)]
        public async Task<JsonResult> ImportContentToClassroom([FromHeader] string accountId,[FromForm] RichContentImport richContent) {
            _logger.LogInformation($"{ nameof(ClassContentController) }.{ nameof(ImportContentToClassroom) }: Service starts.");
            
            var errors = richContent.VerifyRichContent();
            if (errors.Length != 0) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Data = errors });
            
            var isBelonged = await _classroomService.IsClassroomBelongedToThisTeacherByAccountId(accountId, richContent.ClassroomId);
            if (!isBelonged.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isBelonged.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });
            
            var classContent = await _classContentService.GetClassContentByClassroomId(richContent.ClassroomId);
            classContent ??= new ClassContent { ClassroomId = richContent.ClassroomId };

            var uploadedFileIds = richContent.FilesForImport
                                             .Select(file => file.OpenReadStream())
                                             .Select(async stream => await _s3Service.UploadFileToBucket(SharedConstants.TextractBucketName, stream))
                                             .Select(task => task.Result)
                                             .ToArray();

            var textractJobIds = uploadedFileIds
                                 .Select(async fileId => await _textractService.StartTextDetectionJobForFile(fileId))
                                 .Select(task => task.Result)
                                 .ToArray();

            var extractedTexts = textractJobIds
                                 .Select(async jobId => await _textractService.GetExtractedTextsFromTextractJob(jobId))
                                 .SelectMany(task => task.Result)
                                 .ToArray();

            var combinedText = extractedTexts
                               .Select(text => $"{ SharedConstants.ParaOpen }{ text }{ SharedConstants.ParaClose }")
                               .Aggregate((former, latter) => $"{ former }{ latter }");

            classContent.HtmlContent = $"{ SharedConstants.DivOpen }{ combinedText }{ SharedConstants.DivClose }";
            var saveDatabaseResult = !Helpers.IsProperString(classContent.Id)
                ? await _classContentService.InsertNewContent(classContent) is not null
                : await _classContentService.UpdateContent(classContent);
            
            return !saveDatabaseResult.HasValue || !saveDatabaseResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }
        
        /// <summary>
        /// For teacher. To update rich-text content for classroom contents.
        /// </summary>
        /// <remarks>
        /// Request signature:
        ///     PUT /class-content/update-rich-content
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        ///     Body
        ///         {
        ///             classroomId: string,
        ///             htmlContent: string
        ///         }
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="richContent">The required data to be updated.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [HttpPut("update-rich-content")]
        [RoleAuthorize(Role.Teacher)]
        public async Task<JsonResult> UpdateContentForClassroom([FromHeader] string accountId,[FromBody] RichContent richContent) {
            _logger.LogInformation($"{ nameof(ClassContentController) }.{ nameof(UpdateContentForClassroom) }: Service starts.");
            
            var errors = richContent.VerifyRichContent();
            if (errors.Length != 0) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Data = errors });
            
            var isBelonged = await _classroomService.IsClassroomBelongedToThisTeacherByAccountId(accountId, richContent.ClassroomId);
            if (!isBelonged.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isBelonged.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });
            
            var classContent = await _classContentService.GetClassContentByClassroomId(richContent.ClassroomId);
            if (classContent is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            classContent.HtmlContent = richContent.HtmlContent;
            var updateResult = await _classContentService.UpdateContent(classContent);
            
            return !updateResult.HasValue || !updateResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }
        
        /// <summary>
        /// For both. To get all data to display for classroom contents including all files and rich-text content.
        /// </summary>
        /// <remarks>
        /// Request signature:
        ///     GET /class-content/all/{string}
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        ///
        /// Returned object signature:
        /// {
        ///     id: string,
        ///     videos: [FileVM],
        ///     audios: [FileVM],
        ///     photos: [FileVM],
        ///     attachments: [FileVM],
        ///     htmlContent: string
        /// }
        ///
        /// where `<c>FileVM</c>` is an object:
        /// {
        ///     id: string,
        ///     name: string,
        ///     type: 0 | 1 | 2 | 3,
        ///     extension: string,
        ///     uploadedOn: number ---> Unix timestamp
        /// }
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="classroomId">The classroom ID to get contents for.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [HttpGet("all/{classroomId}")]
        public async Task<JsonResult> GetClassroomContents([FromHeader] string accountId,[FromRoute] string classroomId) {
            _logger.LogInformation($"{ nameof(ClassContentController) }.{ nameof(UpdateContentForClassroom) }: Service starts.");
            
            var isBelonged = await _classroomService.IsClassroomBelongedToThisTeacherByAccountId(accountId, classroomId);
            if (!isBelonged.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isBelonged.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });
            
            var classContent = await _classContentService.GetClassContentVmByClassroomId(classroomId);
            return classContent is null
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = classContent });
        }

        private async Task<FileVM[]> UploadVideosToS3Bucket(string classroomId, IFormFileCollection videos) {
            _logger.LogInformation($"private { nameof(ClassContentController) }.{ nameof(UploadVideosToS3Bucket) }: Service starts.");

            var videoBucketName = GetBucketNameForFileType(classroomId, (byte) FileType.video);
            var isBucketCreated = await _s3Service.CreateBucketWithNameIfNeeded(videoBucketName);
            if (!isBucketCreated) return default;

            return videos
                   .Select(video => new {
                       Stream = video.OpenReadStream(),
                       Name = video.FileName,
                       Type = video.ContentType.Split(SharedConstants.FSlash)[^1]
                   })
                   .Select(async video => {
                       var fileId = await _s3Service.UploadFileToBucket(videoBucketName, video.Stream);
                       return new { Id = fileId, video.Name, video.Type };
                   })
                   .Select(task => task.Result)
                   .Select(result => new FileVM {
                       Id = result.Id,
                       Name = result.Name,
                       Type = (byte) FileType.video,
                       Extension = result.Type,
                       UploadedOn = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                   })
                   .ToArray();
        }
        
        private async Task<FileVM[]> UploadAudiosToS3Bucket(string classroomId, IFormFileCollection audios) {
            _logger.LogInformation($"private { nameof(ClassContentController) }.{ nameof(UploadAudiosToS3Bucket) }: Service starts.");

            var audioBucketName = GetBucketNameForFileType(classroomId, (byte) FileType.audio);
            var isBucketCreated = await _s3Service.CreateBucketWithNameIfNeeded(audioBucketName);
            if (!isBucketCreated) return default;

            return audios
                   .Select(audio => new {
                       Stream = audio.OpenReadStream(),
                       Name = audio.FileName,
                       Type = audio.ContentType.Split(SharedConstants.FSlash)[^1]
                   })
                   .Select(async audio => {
                       var fileId = await _s3Service.UploadFileToBucket(audioBucketName, audio.Stream);
                       return new { Id = fileId, audio.Name, audio.Type };
                   })
                   .Select(task => task.Result)
                   .Select(result => new FileVM {
                       Id = result.Id,
                       Name = result.Name,
                       Type = (byte) FileType.audio,
                       Extension = result.Type,
                       UploadedOn = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                   })
                   .ToArray();
        }
        
        private async Task<FileVM[]> UploadPhotosToS3Bucket(string classroomId, IFormFileCollection photos) {
            _logger.LogInformation($"private { nameof(ClassContentController) }.{ nameof(UploadPhotosToS3Bucket) }: Service starts.");

            var photoBucketName = GetBucketNameForFileType(classroomId, (byte) FileType.photo);
            var isBucketCreated = await _s3Service.CreateBucketWithNameIfNeeded(photoBucketName);
            if (!isBucketCreated) return default;

            return photos
                   .Select(photo => new {
                       Stream = photo.OpenReadStream(),
                       Name = photo.FileName,
                       Type = photo.ContentType.Split(SharedConstants.FSlash)[^1]
                   })
                   .Select(async photo => {
                       var fileId = await _s3Service.UploadFileToBucket(photoBucketName, photo.Stream);
                       return new { Id = fileId, photo.Name, photo.Type };
                   })
                   .Select(task => task.Result)
                   .Select(result => new FileVM {
                       Id = result.Id,
                       Name = result.Name,
                       Type = (byte) FileType.photo,
                       Extension = result.Type,
                       UploadedOn = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                   })
                   .ToArray();
        }
        
        private async Task<FileVM[]> UploadAttachmentsToS3Bucket(string classroomId, IFormFileCollection attachments) {
            _logger.LogInformation($"private { nameof(ClassContentController) }.{ nameof(UploadAttachmentsToS3Bucket) }: Service starts.");

            var attachmentBucketName = GetBucketNameForFileType(classroomId, (byte) FileType.other);
            var isBucketCreated = await _s3Service.CreateBucketWithNameIfNeeded(attachmentBucketName);
            if (!isBucketCreated) return default;

            return attachments
                   .Select(attachment => new {
                       Stream = attachment.OpenReadStream(),
                       Name = attachment.FileName,
                       Type = attachment.ContentType.Split(SharedConstants.FSlash)[^1]
                   })
                   .Select(async attachment => {
                       var fileId = await _s3Service.UploadFileToBucket(attachmentBucketName, attachment.Stream);
                       return new { Id = fileId, attachment.Name, attachment.Type };
                   })
                   .Select(task => task.Result)
                   .Select(result => new FileVM {
                       Id = result.Id,
                       Name = result.Name,
                       Type = (byte) FileType.other,
                       Extension = result.Type,
                       UploadedOn = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                   })
                   .ToArray();
        }

        private string GetBucketNameForFileType(string classroomId, byte fileType) {
            return fileType switch {
                (byte) FileType.video => $"{ classroomId }.{ nameof(FileType.video) }s",
                (byte) FileType.audio => $"{ classroomId }.{ nameof(FileType.audio) }s",
                (byte) FileType.photo => $"{ classroomId }.{ nameof(FileType.photo) }s",
                (byte) FileType.other => $"{ classroomId }.{ nameof(FileType.other) }s",
                _ => default
            };
        }
    }
}