using System;
using System.Threading.Tasks;
using COSC2640A3.Attributes;
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
    [TwoFaAuthorize]
    [RoleAuthorize(Role.Student)]
    [Route("student")]
    public sealed class StudentController {
        
        private readonly ILogger<StudentController> _logger;
        private readonly IEnrolmentService _enrolmentService;
        private readonly IAccountService _accountService;
        private readonly IClassroomService _classroomService;
        private readonly IContextService _contextService;

        public StudentController(
            ILogger<StudentController> logger,
            IEnrolmentService enrolmentService,
            IAccountService accountService,
            IContextService contextService,
            IClassroomService classroomService
        ) {
            _logger = logger;
            _enrolmentService = enrolmentService;
            _accountService = accountService;
            _classroomService = classroomService;
            _contextService = contextService;
        }

        [HttpPost("enrol/{classroomId}")]
        public async Task<JsonResult> EnrolIntoClassroom([FromHeader] string accountId,[FromRoute] string classroomId) {
            _logger.LogInformation($"{ nameof(StudentController) }.{ nameof(EnrolIntoClassroom) }: Service starts.");

            var student = await _accountService.GetStudentByAccountId(accountId);
            if (student is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var classroom = await _classroomService.GetClassroomById(classroomId);
            if (classroom is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var enrolment = new Enrolment {
                StudentId = student.Id,
                ClassroomId = classroomId,
                EnrolledOn = DateTime.UtcNow
            };

            await _contextService.StartTransaction();
            var enrolmentId = await _enrolmentService.InsertNewEnrolment(enrolment);

            if (!Helpers.IsProperString(enrolmentId)) {
                await _contextService.RevertTransaction();
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            }

            var invoice = new Invoice { DueAmount = classroom.Price };

            var invoiceId = await _enrolmentService.InsertNewInvoice(invoice);
            if (!Helpers.IsProperString(invoiceId)) {
                await _contextService.RevertTransaction();
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            }

            enrolment.Id = enrolmentId;
            enrolment.InvoiceId = invoiceId;

            var updateEnrolmentResult = await _enrolmentService.UpdateEnrolment(enrolment);
            if (!updateEnrolmentResult.HasValue || !updateEnrolmentResult.Value) {
                await _contextService.RevertTransaction();
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            }

            await _contextService.ConfirmTransaction();
            return new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = enrolmentId });
        }
        
        [HttpDelete("unenrol/{enrolmentId}")]
        public async Task<JsonResult> UnenrolToClassroom([FromHeader] string accountId,[FromRoute] string enrolmentId) {
            _logger.LogInformation($"{ nameof(StudentController) }.{ nameof(UnenrolToClassroom) }: Service starts.");

            var isBelonged = await _enrolmentService.IsEnrolmentMadeByStudentByAccountId(enrolmentId, accountId);
            if (!isBelonged.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isBelonged.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });

            var enrolment = await _enrolmentService.GetEnrolmentById(enrolmentId);
            if (enrolment is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            await _contextService.StartTransaction();
            var deleteEnrolmentResult = await _enrolmentService.DeleteEnrolment(enrolment);
            if (!deleteEnrolmentResult.HasValue || !deleteEnrolmentResult.Value) {
                await _contextService.RevertTransaction();
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            }

            var invoice = await _enrolmentService.GetInvoiceByEnrolmentId(enrolmentId);
            if (invoice is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var deleteInvoiceResult = await _enrolmentService.DeleteInvoice(invoice);
            if (!deleteInvoiceResult.HasValue || !deleteInvoiceResult.Value) {
                await _contextService.RevertTransaction();
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            }

            await _contextService.ConfirmTransaction();
            return new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }
        
        [HttpGet("enrolments")]
        public async Task<JsonResult> GetAllEnrolments([FromHeader] string accountId) {
            _logger.LogInformation($"{ nameof(StudentController) }.{ nameof(GetAllEnrolments) }: Service starts.");

            var enrolments = await _enrolmentService.GetStudentEnrolmentsByAccountId(accountId);
            return enrolments is null
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = enrolments });
        }
    }
}