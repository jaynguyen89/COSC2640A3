using System;
using System.Threading.Tasks;
using COSC2640A3.Attributes;
using COSC2640A3.Bindings;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static Helper.Shared.SharedEnums;

namespace COSC2640A3.Controllers {

    [ApiController]
    [MainAuthorize]
    [TwoFaAuthorize]
    [Route("teacher")]
    public sealed class TeacherController {
        
        private readonly ILogger<TeacherController> _logger;
        private readonly IEnrolmentService _enrolmentService;
        private readonly IAccountService _accountService;

        public TeacherController(
            ILogger<TeacherController> logger,
            IEnrolmentService enrolmentService,
            IAccountService accountService
        ) {
            _logger = logger;
            _enrolmentService = enrolmentService;
            _accountService = accountService;
        }

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

        [HttpPost("export-classrooms")]
        public async Task<JsonResult> ExportClassroomData([FromHeader] string accountId,[FromBody] DataExport dataExport) {
            _logger.LogInformation($"{ nameof(TeacherController) }.{ nameof(ExportClassroomData) }: Service starts.");
            throw new NotImplementedException();
        }
        
        [HttpPost("export-students")]
        public async Task<JsonResult> ExportStudentsInClassroomData([FromHeader] string accountId,[FromBody] DataExport dataExport) {
            _logger.LogInformation($"{ nameof(TeacherController) }.{ nameof(ExportStudentsInClassroomData) }: Service starts.");
            throw new NotImplementedException();
        }
    }
}