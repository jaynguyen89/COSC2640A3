using System;
using System.Threading.Tasks;
using AmazonLibrary.Interfaces;
using AmazonLibrary.Models;
using COSC2640A3.Attributes;
using COSC2640A3.Bindings;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.ViewModels;
using Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Helper.Shared.SharedEnums;

namespace COSC2640A3.Controllers {

    [ApiController]
    [MainAuthorize]
    [Route("classroom")]
    public sealed class ClassroomController {
        
        private readonly ILogger<ClassroomController> _logger;
        private readonly IClassroomService _classroomService;
        private readonly IAccountService _accountService;
        private readonly IRedisCacheService _redisCache;
        private readonly IS3Service _s3Service;
        private readonly IDynamoService _dynamoService;

        public ClassroomController(
            ILogger<ClassroomController> logger,
            IClassroomService classroomService,
            IAccountService accountService,
            IRedisCacheService redisCache,
            IS3Service s3Service,
            IDynamoService dynamoService
        ) {
            _logger = logger;
            _classroomService = classroomService;
            _accountService = accountService;
            _redisCache = redisCache;
            _s3Service = s3Service;
            _dynamoService = dynamoService;
        }

        [RoleAuthorize(Role.Teacher)]
        [HttpPost("create")]
        public async Task<JsonResult> CreateClassroom([FromHeader] string accountId,[FromBody] Classroom classroom) {
            _logger.LogInformation($"{ nameof(ClassroomController) }.{ nameof(CreateClassroom) }: Service starts.");
            
            var errors = classroom.VerifyClassroomData();
            if (errors.Length != 0) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = errors });

            var teacher = await _accountService.GetTeacherByAccountId(accountId);
            if (teacher is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            classroom.TeacherId = teacher.Id;
            classroom.IsActive = true;
            var classroomId = await _classroomService.InsertNewClassroom(classroom);
            
            return !Helpers.IsProperString(classroomId)
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = classroomId });
        }
        
        [RoleAuthorize(Role.Teacher)]
        [HttpPut("update")]
        public async Task<JsonResult> UpdateClassroom([FromHeader] string accountId,[FromBody] Classroom classroom) {
            _logger.LogInformation($"{ nameof(ClassroomController) }.{ nameof(UpdateClassroom) }: Service starts.");

            var isBelonged = await _classroomService.IsClassroomBelongedToThisTeacherByAccountId(accountId, classroom.Id);
            if (!isBelonged.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isBelonged.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });
            
            var errors = classroom.VerifyClassroomData();
            if (errors.Length != 0) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = errors });

            var updateResult = await _classroomService.UpdateClassroom(classroom);
            return !updateResult.HasValue || !updateResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }
        
        [RoleAuthorize(Role.Teacher)]
        [HttpDelete("remove/{classroomId}")]
        public async Task<JsonResult> RemoveClassroom([FromHeader] string accountId,[FromRoute] string classroomId) {
            _logger.LogInformation($"{ nameof(ClassroomController) }.{ nameof(RemoveClassroom) }: Service starts.");
            
            var isBelonged = await _classroomService.IsClassroomBelongedToThisTeacherByAccountId(accountId, classroomId);
            if (!isBelonged.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isBelonged.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });

            var classroom = await _classroomService.GetClassroomById(classroomId);
            if (classroom is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            
            var removeResult = await _classroomService.DeleteClassroom(classroom);
            return !removeResult.HasValue || !removeResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }

        [HttpGet("all-by-teacher/{teacherId}")]
        public async Task<JsonResult> GetAllClassroomsByTeacher([FromHeader] string accountId,[FromRoute] string teacherId) {
            _logger.LogInformation($"{ nameof(ClassroomController) }.{ nameof(GetAllClassroomsByTeacher) }: Service starts.");

            var authenticatedUser = await _redisCache.GetRedisCacheEntry<AuthenticatedUser>($"{ nameof(AuthenticatedUser) }_{ accountId }");
            if (authenticatedUser.Role == Role.Teacher) {
                var teacherByAuthenticatedUser = await _accountService.GetTeacherByAccountId(accountId);
                if (teacherByAuthenticatedUser is null)
                    return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
                
                if (teacherByAuthenticatedUser.Id != teacherId)
                    return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });
            }

            var classrooms = await _classroomService.GetAllClassroomsByTeacherId(teacherId);
            return classrooms is null ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                                      : new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = classrooms });
        }

        [RoleAuthorize(Role.Teacher)]
        [HttpPost("upload")]
        public async Task<JsonResult> UploadFileForImports([FromHeader] string accountId,[FromForm] FileImportUpload uploading) {
            _logger.LogInformation($"{ nameof(ClassroomController) }.{ nameof(UploadFileForImports) }: Service starts.");

            var errors = uploading.VerifyUploading();
            if (errors.Length != 0) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = errors });

            var uploadedFileId = await _s3Service.UploadFileForImportToS3Bucket(uploading.FileForImport.OpenReadStream(), uploading.ImportType.ToString());
            if (!Helpers.IsProperString(uploadedFileId)) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while uploading your file." } });

            var classImportSchedule = new ImportSchedule {
                AccountId = accountId,
                FileId = uploadedFileId,
                FileName = uploading.FileForImport.FileName,
                FileSize = uploading.FileForImport.Length,
                UploadedOn = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Status = (byte) ScheduleStatus.Awaiting
            };

            var scheduleId = await _dynamoService.SaveToSchedulesTable(classImportSchedule, uploading.ImportType.ToString());
            if (!Helpers.IsProperString(scheduleId)) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while uploading your file." } });

            classImportSchedule.Id = scheduleId;
            return new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = classImportSchedule });
        }

        [RoleAuthorize(Role.Teacher)]
        [HttpPut("completed/{classroomId}")]
        public async Task<JsonResult> MarkClassroomAsCompleted([FromHeader] string accountId,[FromRoute] string classroomId) {
            _logger.LogInformation($"{ nameof(ClassroomController) }.{ nameof(MarkClassroomAsCompleted) }: Service starts.");
            
            var isBelonged = await _classroomService.IsClassroomBelongedToThisTeacherByAccountId(accountId, classroomId);
            if (!isBelonged.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isBelonged.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });
            
            var classroom = await _classroomService.GetClassroomById(classroomId);
            if (classroom is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            classroom.IsActive = false;
            var updateResult = await _classroomService.UpdateClassroom(classroom);
            
            return !updateResult.HasValue || !updateResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }

        [RoleAuthorize(Role.Student)]
        [HttpGet("all")]
        public async Task<JsonResult> GetAllClassrooms() {
            _logger.LogInformation($"{ nameof(ClassroomController) }.{ nameof(GetAllClassrooms) }: Service starts.");

            var classrooms = await _classroomService.GetAllClassrooms();
            return classrooms is null
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = classrooms });
        }

        [RoleAuthorize(Role.Teacher)]
        [HttpGet("enrolments/{classroomId}")]
        public async Task<JsonResult> GetEnrolmentsByClassroom([FromHeader] string accountId,[FromRoute] string classroomId) {
            
        }
    }
}