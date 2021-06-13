using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AmazonLibrary.Interfaces;
using AmazonLibrary.Models;
using AssistantLibrary.Interfaces;
using COSC2640A3.Attributes;
using COSC2640A3.Bindings;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.ViewModels;
using COSC2640A3.ViewModels.Account;
using Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Helper.Shared.SharedEnums;

namespace COSC2640A3.Controllers {

    [AppActionFiler]
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

        /// <summary>
        /// For student. Update student data including school name, faculty, and personal URL.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     PUT /account/update-student
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        ///     Body
        ///         {
        ///             "id": string,
        ///             "schoolName": string,
        ///             "faculty": string
        ///             "personalUrl": string
        ///         }
        /// </code>
        /// -->
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="studentDetail">The required data to update Student.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [RoleAuthorize(Role.Student)]
        [HttpPut("update-student")]
        public async Task<JsonResult> UpdateStudentDetails([FromHeader] string accountId,[FromBody] StudentDetail studentDetail) {
            _logger.LogInformation($"{ nameof(AccountController) }.{ nameof(UpdateStudentDetails) }: Service starts.");

            var errors = studentDetail.VerifyDetail();
            if (errors.Length != 0) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = errors });

            var isAssociated = await _accountService.IsStudentInfoAssociatedWithAccount(studentDetail.Id, accountId);
            if (!isAssociated.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isAssociated.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });

            var student = await _accountService.GetStudentByAccountId(accountId);
            if (student is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            
            student.UpdateDetail(studentDetail);

            var updateStudentResult = await _accountService.UpdateStudent(student);
            if (!updateStudentResult.HasValue || !updateStudentResult.Value)
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            return new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }

        /// <summary>
        /// For student. Get all student details including student data and account data.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     GET /account/student
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        /// </code>
        /// -->
        /// 
        /// Returned object signature:
        /// <!--
        /// <code>
        /// {
        ///     email: string,
        ///     username: string,
        ///     phoneNumber: string,
        ///     phoneNumberConfirmed: boolean,
        ///     twoFaEnabled: boolean,
        ///     preferredName: string,
        ///     twoFa: {
        ///         qrImageUrl: string,
        ///         manualQrCode: string
        ///     },
        ///     studentId: string,
        ///     schoolName: string,
        ///     faculty: string,
        ///     personalUrl: string
        /// }
        /// </code>
        /// -->
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string], Data = object }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
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
        
        /// <summary>
        /// For teacher. Update teacher data including company, jobTitle, and personal website.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     PUT /account/update-teacher
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        ///     Body
        ///         {
        ///             "id": string,
        ///             "company": string,
        ///             "jobTitle": string
        ///             "personalUrl": string
        ///         }
        /// </code>
        /// -->
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="teacherDetail">The required data to update Teacher.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [RoleAuthorize(Role.Teacher)]
        [HttpPut("update-teacher")]
        public async Task<JsonResult> UpdateTeacherDetails([FromHeader] string accountId,[FromBody] TeacherDetail teacherDetail) {
            _logger.LogInformation($"{ nameof(AccountController) }.{ nameof(UpdateTeacherDetails) }: Service starts.");
            
            var errors = teacherDetail.VerifyDetail(false);
            if (errors.Length != 0) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = errors });
            
            var isAssociated = await _accountService.IsTeacherInfoAssociatedWithAccount(teacherDetail.Id, accountId);
            if (!isAssociated.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isAssociated.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "You are not authorized for this request." } });

            var teacher = await _accountService.GetTeacherByAccountId(accountId);
            if (teacher is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            
            teacher.UpdateDetail(teacherDetail);

            var updateTeacherResult = await _accountService.UpdateTeacher(teacher);
            if (!updateTeacherResult.HasValue || !updateTeacherResult.Value)
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            return new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }
        
        /// <summary>
        /// For teacher. Get all teacher details including teacher data and account data.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     GET /account/teacher
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        /// </code>
        /// -->
        /// 
        /// Returned object signature:
        /// <!--
        /// <code>
        /// {
        ///     email: string,
        ///     username: string,
        ///     phoneNumber: string,
        ///     phoneNumberConfirmed: boolean,
        ///     twoFaEnabled: boolean,
        ///     preferredName: string,
        ///     twoFa: {
        ///         qrImageUrl: string,
        ///         manualQrCode: string
        ///     },
        ///     teacherId: string,
        ///     company: string,
        ///     jobTitle: string,
        ///     personalWebsite: string
        /// }
        /// </code>
        /// -->
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string], Data = object }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
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

        /// <summary>
        /// For both. To turn on or renew Two-Factor Authentication protection.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     PUT /account/new-tfa
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        /// </code>
        /// -->
        ///
        /// Returned object signature:
        /// <!--
        /// <code>
        /// {
        ///     qrImageUrl: string,
        ///     manualEntryKey: string
        /// }
        /// </code>
        /// -->
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string], Data = object }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [HttpPut("new-tfa")]
        public async Task<JsonResult> EnableOrRenewTwoFa([FromHeader] string accountId) {
            _logger.LogInformation($"{ nameof(AccountController) }.{ nameof(EnableOrRenewTwoFa) }: Service starts.");

            var account = await _accountService.GetAccountById(accountId);
            if (account is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var twoFa = _googleService.ProduceTwoFactorAuthSetup(account.EmailAddress);
            account.TwoFaSecretKey = twoFa.SecretKey;
            account.TwoFactorEnabled = true;

            var updateAccountResult = await _accountService.UpdateAccount(account);
            if (!updateAccountResult.HasValue || !updateAccountResult.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            
            return new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = (TwoFaVM) twoFa });
        }

        /// <summary>
        /// For both. To turn off Two-Factor Authentication protection.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     PUT /account/disable-tfa/{string}
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        /// </code>
        /// -->
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="recaptchaToken" type="string">The recaptcha confirmation, not required in testings.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [HttpPut("disable-tfa/{recaptchaToken}")]
        public async Task<JsonResult> DisableTwoFa([FromHeader] string accountId,[FromRoute] string recaptchaToken) {
            _logger.LogInformation($"{ nameof(AccountController) }.{ nameof(DisableTwoFa) }: Service starts.");
            
            var isHuman = await _googleService.IsHumanInteraction(recaptchaToken);
            if (!isHuman.Result) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "Recaptcha verification failed." } });
            
            var account = await _accountService.GetAccountById(accountId);
            if (account is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            account.TwoFaSecretKey = default;
            account.TwoFactorEnabled = false;
            var updateAccountResult = await _accountService.UpdateAccount(account);

            await SendEmail(new EmailComposer {
                ReceiverEmail = account.EmailAddress,
                EmailType = EmailType.TwoFaDisabledNotification,
                Subject = $"{ nameof(COSC2640A3) } - Two FA Disabled",
                Contents = new Dictionary<string, string> {
                    { "USER_NAME_PLACEHOLDER", account.Username },
                    { "WEBSITE_URL_PLACEHOLDER", Helper.Shared.SharedConstants.ClientUrl }
                }
            });

            return !updateAccountResult.HasValue || !updateAccountResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }

        /// <summary>
        /// For both. To add a phone number for an account. An SMS will be sent to the phone number to confirm it so use a <b>REAL</b> phone number.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     PUT /account/set-phone-number/{string}
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        /// </code>
        /// -->
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="phoneNumber" type="string">The phone number to be added.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [HttpPut("set-phone-number/{phoneNumber}")]
        public async Task<JsonResult> UpdatePhoneNumber([FromHeader] string accountId,[FromRoute] string phoneNumber) {
            _logger.LogInformation($"{ nameof(AccountController) }.{ nameof(UpdatePhoneNumber) }: Service starts.");
            
            if (!Helpers.IsProperString(phoneNumber)) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { $"{ nameof(Account.PhoneNumber).ToHumanStyled() } is missing." } });
            
            phoneNumber = phoneNumber.Trim().RemoveAllSpaces();
            if (!new Regex(@"^[0-9]{10,15}$").IsMatch(phoneNumber)) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { $"{ nameof(Account.PhoneNumber).ToHumanStyled() } is invalid." } });
            
            var account = await _accountService.GetAccountById(accountId);
            if (account is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            account.PhoneNumber = phoneNumber;
            var updateResult = await _accountService.UpdateAccount(account);
            if (!updateResult.HasValue || !updateResult.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            
            return await GenerateNewAccountTokenFor(account, NotificationType.Sms);
        }

        /// <summary>
        /// For both. To remove phone number for an account.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     PUT /account/remove-phone-number
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        /// </code>
        /// -->
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
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