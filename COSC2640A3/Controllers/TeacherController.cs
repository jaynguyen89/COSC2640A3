using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using AmazonLibrary.Interfaces;
using COSC2640A3.Attributes;
using COSC2640A3.Bindings;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static Helper.Shared.SharedEnums;

namespace COSC2640A3.Controllers {

    [AppActionFiler]
    [ApiController]
    [MainAuthorize]
    [TwoFaAuthorize]
    [Route("teacher")]
    public sealed class TeacherController : ControllerBase {
        
        private readonly ILogger<TeacherController> _logger;
        private readonly IEnrolmentService _enrolmentService;
        private readonly IAccountService _accountService;
        private readonly IClassroomService _classroomService;
        private readonly IDynamoService _dynamoService;

        public TeacherController(
            ILogger<TeacherController> logger,
            IEnrolmentService enrolmentService,
            IAccountService accountService,
            IClassroomService classroomService,
            IDynamoService dynamoService
        ) {
            _logger = logger;
            _enrolmentService = enrolmentService;
            _accountService = accountService;
            _classroomService = classroomService;
            _dynamoService = dynamoService;
        }

        /// <summary>
        /// For teacher. To add or update marks for a student enrolment.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     POST /teacher/add-marks
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        ///     Body
        ///         {
        ///             enrolmentId: string,
        ///             markBreakdowns: [MarkBreakdownVM]
        ///         }
        /// </code>
        /// -->
        ///
        /// where `<c>MarkBreakdownVM</c>` has following schema:
        /// <!--
        /// <code>
        /// {
        ///     taskName: string,
        ///     totalMarks: number,
        ///     rewardedMarks: number,
        ///     markedOn: string,
        ///     comment: string
        /// }
        /// </code>
        /// -->
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="studentMarks">The details od marking to update enrolment.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [HttpPost("add-marks")]
        public async Task<JsonResult> AddMarksToStudentEnrolment([FromHeader] string accountId,[FromBody] StudentMarks studentMarks) {
            _logger.LogInformation($"{ nameof(TeacherController) }.{ nameof(AddMarksToStudentEnrolment) }: Service starts.");

            var errors = studentMarks.VerifyStudentMarks();
            if (errors.Length != 0) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = errors });

            var teacher = await _accountService.GetTeacherByAccountId(accountId);
            if (teacher is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            
            var hasAssociation = await _enrolmentService.DoesEnrolmentRelateToAClassroomOfThisTeacher(studentMarks.EnrolmentId, teacher.Id);
            if (!hasAssociation.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!hasAssociation.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });

            var enrolment = await _enrolmentService.GetEnrolmentById(studentMarks.EnrolmentId);
            if (enrolment is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            enrolment.MarkBreakdowns = JsonConvert.SerializeObject(studentMarks.MarkBreakdowns);
            enrolment.OverallMark = (byte) studentMarks.CalculateOverallMarks();

            var updateEnrolmentResult = await _enrolmentService.UpdateEnrolment(enrolment);
            return !updateEnrolmentResult.HasValue || !updateEnrolmentResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }

        /// <summary>
        /// For teacher. To export selected classrooms to a JSON file and download.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     POST /teacher/export-classrooms
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        ///     Body
        ///         {
        ///             classroomIds: [string]
        ///         }
        /// </code>
        /// -->
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="dataExport">The IDs of classrooms to be exported.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [HttpPost("export-classrooms")]
        public async Task<ActionResult> ExportClassroomData([FromHeader] string accountId,[FromBody] DataExport dataExport) {
            _logger.LogInformation($"{ nameof(TeacherController) }.{ nameof(ExportClassroomData) }: Service starts.");

            var isBelonged = await _classroomService.AreTheseClassroomsBelongedTo(accountId, dataExport.ClassroomIds);
            if (!isBelonged.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isBelonged.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });

            var exportedData = await _classroomService.GetClassroomDataForExportBy(dataExport.ClassroomIds);
            if (exportedData is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var exportedFile = new MemoryStream();
            var writer = new StreamWriter(exportedFile);
            await writer.WriteLineAsync(JsonConvert.SerializeObject(exportedData));
            await writer.FlushAsync();

            exportedFile.Position = 0;
            return File(exportedFile, MediaTypeNames.Text.Plain, $"{ accountId }_classrooms_exports.json");
        }
        
        /// <summary>
        /// For teacher. To export student enrolment and invoice data from selected classroom to a JSON file and download.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     POST /teacher/export-students
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        ///     Body
        ///         {
        ///             classroomIds: [string]
        ///         }
        /// </code>
        /// -->
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="dataExport">The IDs of classrooms to be exported.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [HttpPost("export-students")]
        public async Task<ActionResult> ExportStudentsInClassroomData([FromHeader] string accountId,[FromBody] DataExport dataExport) {
            _logger.LogInformation($"{ nameof(TeacherController) }.{ nameof(ExportStudentsInClassroomData) }: Service starts.");
            
            var isBelonged = await _classroomService.AreTheseClassroomsBelongedTo(accountId, dataExport.ClassroomIds);
            if (!isBelonged.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isBelonged.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });

            var exportedData = await _enrolmentService.GetEnrolmentDataForExportBy(dataExport.ClassroomIds);
            if (exportedData is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var exportedFile = new MemoryStream();
            var writer = new StreamWriter(exportedFile);
            await writer.WriteLineAsync(JsonConvert.SerializeObject(exportedData));
            await writer.FlushAsync();

            exportedFile.Position = 0;
            return File(exportedFile, MediaTypeNames.Text.Plain, $"{ accountId }_enrolments_exports.json");
        }

        /// <summary>
        /// For teacher. To get all Lambda schedules from DynamoDb.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     GET /teacher/schedules
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        /// </code>
        /// -->
        /// 
        /// Returned object signature:
        /// - <c>fileSize</c> is in KB (integer)
        /// - <c>uploadedOn</c> is a Unix timestamp
        /// <!--
        /// <code>
        /// [{
        ///     id: string,
        ///     accountId: string,
        ///     fileId: string,
        ///     fileName: string,
        ///     fileSize: number,
        ///     uploadedOn: number,
        ///     status: 0 | 1 | 2 | 3 | 4,
        ///     isForClassroom: boolean
        /// }]
        /// </code>
        /// -->
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string], Data = object }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [HttpGet("schedules")]
        public async Task<JsonResult> GetImportSchedules([FromHeader] string accountId) {
            _logger.LogInformation($"{ nameof(TeacherController) }.{ nameof(GetImportSchedules) }: Service starts.");

            var schedules = await _dynamoService.GetAllSchedulesDataFor(accountId);
            return schedules is null
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = schedules });
        }
    }
}