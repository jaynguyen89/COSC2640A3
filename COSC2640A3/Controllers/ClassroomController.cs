using System;
using System.Threading.Tasks;
using AmazonLibrary.Interfaces;
using AmazonLibrary.Models;
using COSC2640A3.Attributes;
using COSC2640A3.Bindings;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.ViewModels;
using COSC2640A3.ViewModels.Features;
using Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Helper.Shared.SharedEnums;

namespace COSC2640A3.Controllers {

    [ApiController]
    [MainAuthorize]
    [TwoFaAuthorize]
    [Route("classroom")]
    public sealed class ClassroomController {
        
        private readonly ILogger<ClassroomController> _logger;
        private readonly IOptions<MainOptions> _options;
        private readonly IContextService _contextService;
        private readonly IClassroomService _classroomService;
        private readonly IAccountService _accountService;
        private readonly IEnrolmentService _enrolmentService;
        private readonly IRedisCacheService _redisCache;
        private readonly IS3Service _s3Service;
        private readonly IDynamoService _dynamoService;

        public ClassroomController(
            ILogger<ClassroomController> logger,
            IOptions<MainOptions> options,
            IContextService contextService,
            IClassroomService classroomService,
            IAccountService accountService,
            IEnrolmentService enrolmentService,
            IRedisCacheService redisCache,
            IS3Service s3Service,
            IDynamoService dynamoService
        ) {
            _logger = logger;
            _options = options;
            _contextService = contextService;
            _classroomService = classroomService;
            _accountService = accountService;
            _enrolmentService = enrolmentService;
            _redisCache = redisCache;
            _s3Service = s3Service;
            _dynamoService = dynamoService;
        }

        /// <summary>
        /// For teacher. To create a new classroom. Returns ID of the newly created classroom.
        /// </summary>
        /// <remarks>
        /// Request signature:
        ///     POST /classroom/create
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        ///     Body
        ///         {
        ///             className: string,
        ///             capacity: number, ---> lower than 32767 + 1
        ///             price: number, ---> lower than 9999.99
        ///             commencedOn: string,
        ///             duration: number, ---> lower than 255 + 1
        ///             durationUnit: 0 | 1 | 2 | 3 | 4
        ///         }
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="classroom">The required data to create new classroom.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string], Data = string }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
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
        
        /// <summary>
        /// For teacher. To update a classroom.
        /// </summary>
        /// <remarks>
        /// Request signature:
        ///     PUT /classroom/update
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        ///     Body
        ///         {
        ///             className: string,
        ///             capacity: number, ---> lower than 32767 + 1
        ///             price: number, ---> lower than 9999.99
        ///             commencedOn: string,
        ///             duration: number, ---> lower than 255 + 1
        ///             durationUnit: 0 | 1 | 2 | 3 | 4
        ///         }
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="classroom">The required data to update classroom.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [RoleAuthorize(Role.Teacher)]
        [HttpPut("update")]
        public async Task<JsonResult> UpdateClassroom([FromHeader] string accountId,[FromBody] Classroom classroom) {
            _logger.LogInformation($"{ nameof(ClassroomController) }.{ nameof(UpdateClassroom) }: Service starts.");

            var isBelonged = await _classroomService.IsClassroomBelongedToThisTeacherByAccountId(accountId, classroom.Id);
            if (!isBelonged.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isBelonged.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });
            
            var errors = classroom.VerifyClassroomData();
            if (errors.Length != 0) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = errors });

            var teacher = await _accountService.GetTeacherByAccountId(accountId);
            if (teacher is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            classroom.TeacherId = teacher.Id;
            classroom.IsActive = true;
            
            var updateResult = await _classroomService.UpdateClassroom(classroom);
            return !updateResult.HasValue || !updateResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }
        
        /// <summary>
        /// For teacher. To delete a classroom.
        /// </summary>
        /// <remarks>
        /// Request signature:
        ///     DELETE /classroom/remove/{string}
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="classroomId">The ID of classroom to be deleted.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
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
        
        /// <summary>
        /// For both. To get all classrooms owned by a teacher, with basic classroom data in Classroom table.
        /// Situation 1: Students browse classrooms owned by a Teacher.
        /// Situation 2: A Teacher browse all classrooms owned by themselves.
        /// </summary>
        /// <remarks>
        /// Request signature:
        ///     GET /classroom/all-by-teacher/{string}
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        ///
        /// Returned object signature:
        /// {
        ///     classrooms : [ClassroomVM],
        ///     completedClassrooms: [ClassroomVm]
        /// }
        ///
        /// where `<c>ClassroomVM</c>` has the following schema:
        /// {
        ///     id: string,
        ///     teacherId: string,
        ///     teacherName: string,
        ///     className: string,
        ///     price: number,
        ///     enrolmentsCount: number,
        ///     classroomDetail: null
        /// }
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="teacherId">The ID of classroom to be deleted.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string], Data = object }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [HttpGet("all-by-teacher/{teacherId?}")]
        public async Task<JsonResult> GetAllClassroomsByTeacher([FromHeader] string accountId,[FromRoute] string teacherId) {
            _logger.LogInformation($"{ nameof(ClassroomController) }.{ nameof(GetAllClassroomsByTeacher) }: Service starts.");

            ClassroomVM[] classrooms;
            ClassroomVM[] completedClassrooms = null;

            var authenticatedUser = await _redisCache.GetRedisCacheEntry<AuthenticatedUser>($"{ nameof(AuthenticatedUser) }_{ accountId }");
            if (authenticatedUser.Role == Role.Teacher) { // Situation 2
                var teacherByAuthenticatedUser = await _accountService.GetTeacherByAccountId(accountId);
                if (teacherByAuthenticatedUser is null)
                    return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
                
                classrooms = await _classroomService.GetAllClassroomsByTeacherId(teacherByAuthenticatedUser.Id);
                completedClassrooms = await _classroomService.GetAllClassroomsByTeacherId(teacherByAuthenticatedUser.Id, false);
            }
            else // Situation 1
                classrooms = await _classroomService.GetAllClassroomsByTeacherId(teacherId);
            
            return classrooms is null ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                                      : new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = new { classrooms, completedClassrooms } });
        }

        /// <summary>
        /// For teacher. To upload JSON files for importing classroom and student data into database by Lambda apps.
        /// </summary>
        /// <remarks>
        /// Request signature:
        ///     POST /classroom/import
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        ///         "Content-Type": "multipart/form-data"
        ///     Body
        ///         {
        ///             importType: 0 | 1,
        ///             fileForImport: binary
        ///         }
        ///
        /// Returned object signature:
        /// {
        ///     id: string,
        ///     accountId: string,
        ///     fileId: string,
        ///     fileName: string,
        ///     fileSize: number, ---> in KB
        ///     uploadedOn: number, ---> Unix timestamp
        ///     status: 0 | 1 | 2 | 3 | 4,
        ///     isForClassroom: boolean
        /// }
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="uploading">The uploaded JSON files required for importing data.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string], Data = object }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [RoleAuthorize(Role.Teacher)]
        [HttpPost("import")]
        public async Task<JsonResult> UploadFileForImports([FromHeader] string accountId,[FromForm] FileImportUpload uploading) {
            _logger.LogInformation($"{ nameof(ClassroomController) }.{ nameof(UploadFileForImports) }: Service starts.");

            var errors = uploading.VerifyUploading();
            if (errors.Length != 0) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = errors });

            var uploadedFileId = await _s3Service.UploadFileForImportToS3Bucket(uploading.FileForImport.OpenReadStream(), uploading.ImportType);
            if (!Helpers.IsProperString(uploadedFileId)) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while uploading your file." } });

            var classImportSchedule = new ImportSchedule {
                AccountId = accountId,
                FileId = uploadedFileId,
                FileName = uploading.FileForImport.FileName,
                FileSize = uploading.FileForImport.Length,
                UploadedOn = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Status = (byte) ScheduleStatus.Awaiting
            };

            var scheduleId = await _dynamoService.SaveToSchedulesTable(classImportSchedule, uploading.ImportType);
            if (!Helpers.IsProperString(scheduleId)) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while uploading your file." } });

            classImportSchedule.Id = scheduleId;
            return new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = classImportSchedule });
        }

        /// <summary>
        /// For teacher. To mark a classroom as completed. Student's results and invoices will be finalized during the process.
        /// </summary>
        /// <remarks>
        /// Request signature:
        ///     PUT /classroom/completed/{string}
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="classroomId">The ID of classroom to mark as completed.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string], Data = object }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [RoleAuthorize(Role.Teacher)]
        [HttpPut("completed/{classroomId}")]
        public async Task<JsonResult> MarkClassroomAsCompleted([FromHeader] string accountId,[FromRoute] string classroomId) {
            _logger.LogInformation($"{ nameof(ClassroomController) }.{ nameof(MarkClassroomAsCompleted) }: Service starts.");
            
            var isBelonged = await _classroomService.IsClassroomBelongedToThisTeacherByAccountId(accountId, classroomId);
            if (!isBelonged.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isBelonged.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });

            var enrolments = await _classroomService.GetEnrolmentsByClassroomId(classroomId);
            if (enrolments is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            foreach (var enrolment in enrolments) enrolment.IsPassed = enrolment.OverallMark >= byte.Parse(_options.Value.StudentPassMark);

            await _contextService.StartTransaction();
            var updateMultipleEnrolmentResult = await _enrolmentService.UpdateMultipleEnrolments(enrolments);
            if (!updateMultipleEnrolmentResult.HasValue || !updateMultipleEnrolmentResult.Value) {
                await _contextService.RevertTransaction();
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            }

            var classroom = await _classroomService.GetClassroomById(classroomId);
            if (classroom is null) {
                await _contextService.RevertTransaction();
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            }

            classroom.IsActive = false;
            var updateResult = await _classroomService.UpdateClassroom(classroom);
            if (!updateResult.HasValue || !updateResult.Value) {
                await _contextService.RevertTransaction();
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            }

            await _contextService.ConfirmTransaction();
            return new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }

        /// <summary>
        /// For student. To get all classrooms for browsing, excluding the ones created by its teacher role, and the ones they already enrolled in.
        /// Classrooms only have basic data from the Classroom table.
        /// </summary>
        /// <remarks>
        /// Request signature:
        ///     GET /classroom/all
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        ///
        /// Returned object signature:
        /// [{
        ///     id: string,
        ///     teacherId: string,
        ///     teacherName: string,
        ///     className: string,
        ///     price: number,
        ///     enrolmentsCount: number,
        ///     classroomDetail: null
        /// }]
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string], Data = object }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [RoleAuthorize(Role.Student)]
        [HttpGet("all")]
        public async Task<JsonResult> GetAllClassrooms([FromHeader] string accountId) {
            _logger.LogInformation($"{ nameof(ClassroomController) }.{ nameof(GetAllClassrooms) }: Service starts.");
            
            var teacher = await _accountService.GetTeacherByAccountId(accountId);
            if (teacher is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var enrolledClassroomIds = await _classroomService.GetIdsOfClassroomsAlreadyEnrolledByStudentWith(accountId);
            if (enrolledClassroomIds is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var classrooms = await _classroomService.GetAllClassroomsExcludingFrom(teacher.Id, enrolledClassroomIds);
            return classrooms is null
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = classrooms });
        }

        /// <summary>
        /// For teacher. To get all enrolments for a classroom created by themself.
        /// Enrolments have all details from Enrolment table, Student table, and Invoice table.
        /// </summary>
        /// <remarks>
        /// Request signature:
        ///     GET /classroom/enrolments/{string}
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        ///
        /// Returned object signature:
        /// {
        ///     id: string,
        ///     student: {
        ///         email: string,
        ///         username: string,
        ///         phoneNumber: string,
        ///         phoneNumberConfirmed: false,
        ///         twoFaEnabled: false,
        ///         twoFa: null,
        ///         preferredName: string,
        ///         studentId: string,
        ///         schoolName: string,
        ///         faculty: string,
        ///         personalUrl: string
        ///     },
        ///     classroom: null,
        ///     invoice: {
        ///         id: string,
        ///         amount: number,
        ///         isPaid: boolean, ---> if <c>true</c>, <c>paymentDetail</c> will have value
        ///         paymentDetail: PaymentDetailVM | null
        ///     },
        ///     marksDetail: {
        ///         overallMarks: number | null,
        ///         markBreakdowns: [MarkBreakdownVM]
        ///     },
        ///     enrolledOn: string
        /// }
        ///
        /// where `<c>PaymentDetailVM</c>` has following schema:
        /// {
        ///     paymentMethod: string,
        ///     paymentId: string,
        ///     transactionId: string,
        ///     chargeId: string,
        ///     paymentStatus: string,
        ///     paidOn: string
        /// }
        ///
        /// and `<c>MarkBreakdownVM</c>` has following schema:
        /// {
        ///     taskName: string,
        ///     totalMarks: number,
        ///     rewardedMarks: number,
        ///     markedOn: string,
        ///     comment: string
        /// }
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="classroomId" type="string">The ID of classrooms to get all enrolments.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string], Data = object }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [RoleAuthorize(Role.Teacher)]
        [HttpGet("enrolments/{classroomId}")]
        public async Task<JsonResult> GetEnrolmentsByClassroom([FromHeader] string accountId,[FromRoute] string classroomId) {
            _logger.LogInformation($"{ nameof(ClassroomController) }.{ nameof(GetEnrolmentsByClassroom) }: Service starts.");
            
            var isBelonged = await _classroomService.IsClassroomBelongedToThisTeacherByAccountId(accountId, classroomId);
            if (!isBelonged.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isBelonged.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });

            var students = await _classroomService.GetStudentEnrolmentsByClassroomId(classroomId);
            return students is null
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = students });
        }

        /// <summary>
        /// For both. To get all details for a classroom.
        /// </summary>
        /// <remarks>
        /// Request signature:
        ///     GET /classroom/details/{string}
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        ///
        /// Returned object signature:
        /// {
        ///     id: string,
        ///     teacherId: string,
        ///     teacherName: string,
        ///     className: string,
        ///     price: number,
        ///     enrolmentsCount: number,
        ///     classroomDetail: {
        ///         capacity: number,
        ///         commencedOn: string,
        ///         duration: number,
        ///         durationUnit: 0 | 1 | 2 | 3 | 4,
        ///         isActive: boolean,
        ///         createdOn: string,
        ///         normalizedDuration: string
        ///     }
        /// }
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="classroomId" type="string">The ID of classroom to get all details.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string], Data = object }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [HttpGet("details/{classroomId}")]
        public async Task<JsonResult> GetClassroomDetails([FromRoute] string classroomId) {
            _logger.LogInformation($"{ nameof(ClassroomController) }.{ nameof(GetClassroomDetails) }: Service starts.");
            
            var classroomDetail = await _classroomService.GetClassroomDetailsFor(classroomId);
            return classroomDetail is null
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = classroomDetail });
        }
    }
}