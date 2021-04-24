using System.Threading.Tasks;
using AssistantLibrary.Interfaces;
using COSC2640A3.Attributes;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.ViewModels;
using COSC2640A3.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Helper.Shared.SharedEnums;

namespace COSC2640A3.Controllers {

    [ApiController]
    [MainAuthorize]
    [Route("account")]
    public sealed class AccountController {
        
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAccountService _accountService;
        private readonly IGoogleService _googleService;

        public AccountController(
            ILogger<AuthenticationController> logger,
            IAccountService accountService,
            IGoogleService googleService
        ) {
            _logger = logger;
            _accountService = accountService;
            _googleService = googleService;
        }

        [RoleAuthorize(Role.Student)]
        [HttpPut("update-student")]
        public async Task<JsonResult> UpdateStudentDetails(Student student) {
            _logger.LogInformation($"{ nameof(AccountController) }.{ nameof(UpdateStudentDetails) }: Service starts.");

            var updateResult = await _accountService.UpdateStudent(student);
            return !updateResult.HasValue || !updateResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }

        [RoleAuthorize(Role.Student)]
        [HttpGet("student")]
        public async Task<JsonResult> GetStudentDetails([FromHeader] string accountId) {
            _logger.LogInformation($"{ nameof(AccountController) }.{ nameof(GetStudentDetails) }: Service starts.");

            var studentWithAccountData = await _accountService.GetStudentByAccountId(accountId);
            if (studentWithAccountData is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var studentVm = (StudentVM) studentWithAccountData;
            if (!studentVm.TwoFaEnabled) return new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = studentVm });
            
            var twoFaData = _googleService.ReproduceTwoFaAuth(studentWithAccountData.Account.TwoFaSecretKey, studentWithAccountData.Account.EmailAddress);
            studentVm.TwoFa = new TwoFaVM {
                QrImageUrl = twoFaData.QrCodeImageUrl,
                ManualQrCode = twoFaData.ManualEntryKey
            };

            return new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = studentVm });
        }
        
        [RoleAuthorize(Role.Teacher)]
        [HttpPut("update-teacher")]
        public async Task<JsonResult> UpdateTeacherDetails(Teacher teacher) {
            _logger.LogInformation($"{ nameof(AccountController) }.{ nameof(UpdateTeacherDetails) }: Service starts.");
            
            var updateResult = await _accountService.UpdateTeacher(teacher);
            return !updateResult.HasValue || !updateResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }
        
        [RoleAuthorize(Role.Teacher)]
        [HttpGet("teacher")]
        public async Task<JsonResult> GetTeacherDetails([FromHeader] string accountId) {
            _logger.LogInformation($"{ nameof(AccountController) }.{ nameof(GetTeacherDetails) }: Service starts.");

            var teacherWithAccountData = await _accountService.GetTeacherByAccountId(accountId);
            if (teacherWithAccountData is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var teacherVm = (TeacherVM) teacherWithAccountData;
            if (!teacherVm.TwoFaEnabled) return new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = teacherVm });
            
            var twoFaData = _googleService.ReproduceTwoFaAuth(teacherWithAccountData.Account.TwoFaSecretKey, teacherWithAccountData.Account.EmailAddress);
            teacherVm.TwoFa = new TwoFaVM {
                QrImageUrl = twoFaData.QrCodeImageUrl,
                ManualQrCode = twoFaData.ManualEntryKey
            };

            return new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = teacherVm });
        }

        [HttpPut("new-tfa")]
        public async Task<JsonResult> EnableOrRenewTwoFa([FromHeader] string accountId) {
            _logger.LogInformation($"{ nameof(AccountController) }.{ nameof(EnableOrRenewTwoFa) }: Service starts.");

            var account = await _accountService.GetAccountById(accountId);
            if (account is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var twoFa = _googleService.ProduceTwoFactorAuthSetup(account.EmailAddress);
            account.TwoFaSecretKey = twoFa.SecretKey;

            var updateAccountResult = await _accountService.UpdateAccount(account);
            if (!updateAccountResult.HasValue || !updateAccountResult.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            twoFa.SecretKey = default;
            return new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = twoFa });
        }

        [HttpPut("disable-tfa/{recaptchaToken}")]
        public async Task<JsonResult> DisableTwoFa([FromHeader] string accountId,[FromRoute] string recaptchaToken) {
            _logger.LogInformation($"{ nameof(AccountController) }.{ nameof(DisableTwoFa) }: Service starts.");
            
            var isHuman = await _googleService.IsHumanRegistration(recaptchaToken);
            if (!isHuman.Result) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "Recaptcha verification failed." } });
            
            var account = await _accountService.GetAccountById(accountId);
            if (account is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            account.TwoFaSecretKey = default;
            var updateAccountResult = await _accountService.UpdateAccount(account);

            return !updateAccountResult.HasValue || !updateAccountResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }
    }
}