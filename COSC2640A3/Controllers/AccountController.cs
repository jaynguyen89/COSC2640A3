using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AmazonLibrary.Interfaces;
using AmazonLibrary.Models;
using AssistantLibrary.Interfaces;
using COSC2640A3.Attributes;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.ViewModels;
using COSC2640A3.ViewModels.Account;
using Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Helper.Shared.SharedEnums;

namespace COSC2640A3.Controllers {

    [ApiController]
    [MainAuthorize]
    [TwoFaAuthorize]
    [Route("account")]
    public sealed class AccountController : AppController {
        
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IGoogleService _googleService;

        public AccountController(
            ILogger<AuthenticationController> logger,
            IContextService contextService,
            IAccountService accountService,
            IGoogleService googleService,
            ISmsService smsService,
            IAmazonMailService mailService
        ): base(
            contextService, accountService, null, smsService, mailService
        ) {
            _logger = logger;
            _googleService = googleService;
        }

        [RoleAuthorize(Role.Student)]
        [HttpPut("update-student")]
        public async Task<JsonResult> UpdateStudentDetails([FromHeader] string accountId,[FromBody] Student student) {
            _logger.LogInformation($"{ nameof(AccountController) }.{ nameof(UpdateStudentDetails) }: Service starts.");

            var isAssociated = await _accountService.IsStudentInfoAssociatedWithAccount(student.Id, accountId);
            if (!isAssociated.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isAssociated.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });

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
        public async Task<JsonResult> UpdateTeacherDetails([FromHeader] string accountId,[FromBody] Teacher teacher) {
            _logger.LogInformation($"{ nameof(AccountController) }.{ nameof(UpdateTeacherDetails) }: Service starts.");
            
            var isAssociated = await _accountService.IsTeacherInfoAssociatedWithAccount(teacher.Id, accountId);
            if (!isAssociated.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isAssociated.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });
            
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
            
            var isHuman = await _googleService.IsHumanInteraction(recaptchaToken);
            if (!isHuman.Result) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "Recaptcha verification failed." } });
            
            var account = await _accountService.GetAccountById(accountId);
            if (account is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            account.TwoFaSecretKey = default;
            var updateAccountResult = await _accountService.UpdateAccount(account);

            await SendEmail(new EmailComposer {
                ReceiverEmail = account.EmailAddress,
                EmailType = EmailType.TwoFaDisabledNotification,
                Subject = $"{ nameof(COSC2640A3) } - Two FA Disabled",
                Contents = new Dictionary<string, string> {
                    { "USER_NAME_PLACEHOLDER", account.Username },
                    { "WEBSITE_URL_PLACEHOLDER", "http://localhost:3000" }
                }
            });

            return !updateAccountResult.HasValue || !updateAccountResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }

        [HttpPut("set-phone-number/{phoneNumber}")]
        public async Task<JsonResult> UpdatePhoneNumber([FromHeader] string accountId,[FromRoute] string phoneNumber) {
            _logger.LogInformation($"{ nameof(AccountController) }.{ nameof(UpdatePhoneNumber) }: Service starts.");
            
            if (!Helpers.IsProperString(phoneNumber)) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { $"{ nameof(Account.PhoneNumber).ToHumanStyled() } is missing." } });
            
            phoneNumber = phoneNumber.Trim().RemoveAllSpaces();
            if (!new Regex(@"^[0-9]{10,15}$").IsMatch(phoneNumber)) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { $"{ nameof(Account.PhoneNumber).ToHumanStyled() } is invalid." } });
            
            var account = await _accountService.GetAccountById(accountId);
            if (account is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            return await GenerateNewAccountTokenFor(account, NotificationType.Sms);
        }

        [HttpPut("remove-phone-number")]
        public async Task<JsonResult> RemovePhoneNumber([FromHeader] string accountId) {
            _logger.LogInformation($"{ nameof(AccountController) }.{ nameof(RemovePhoneNumber) }: Service starts.");
            
            var account = await _accountService.GetAccountById(accountId);
            if (account is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            account.PhoneNumber = null;
            account.PhoneNumberConfirmed = false;
            var updateAccountResult = await _accountService.UpdateAccount(account);
            
            return !updateAccountResult.HasValue || !updateAccountResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }
    }
}