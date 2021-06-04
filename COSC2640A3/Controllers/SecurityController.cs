using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AmazonLibrary.Interfaces;
using AmazonLibrary.Models;
using AssistantLibrary.Interfaces;
using COSC2640A3.Attributes;
using COSC2640A3.Bindings;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.ViewModels;
using Helper.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Helper.Shared.SharedEnums;

namespace COSC2640A3.Controllers {

    [ApiController]
    [Route("security")]
    public class SecurityController : AppController {

        private readonly ILogger<SecurityController> _logger;
        private readonly IGoogleService _googleService;
        private readonly IAuthenticationService _authenticationService;

        private readonly int _tokenDuration;

        public SecurityController(
            ILogger<SecurityController> logger,
            IGoogleService googleService,
            IAccountService accountService,
            IRedisCacheService redisCache,
            ISmsService smsService,
            IAmazonMailService mailService,
            IContextService contextService,
            IAuthenticationService authenticationService,
            IOptions<MainOptions> options
        ) : base(
            contextService, accountService, redisCache, smsService, mailService
        ) {
            _logger = logger;
            _googleService = googleService;
            _authenticationService = authenticationService;
            _tokenDuration = int.Parse(options.Value.TokenValidityDuration);
        }

        /// <summary>
        /// For both. To verify Two-Factor Authentication after user performs login.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     GET /security/confirm-tfa-pin/{string}
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        /// </code>
        /// -->
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="tfaPin">The Two-Factor PIN to verify.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [HttpGet("confirm-tfa-pin/{tfaPin}")]
        [MainAuthorize]
        public async Task<JsonResult> ConfirmTwoFaPin([FromHeader] string accountId,[FromRoute] string tfaPin) {
            _logger.LogInformation($"{ nameof(SecurityController) }.{ nameof(ConfirmTwoFaPin) }: service starts.");

            var account = await _accountService.GetAccountById(accountId);
            if (account is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            
            var isTfaValid = _googleService.VerifyTwoFactorAuth(account.TwoFaSecretKey, tfaPin);
            if (!isTfaValid) {
                _logger.LogInformation($"{ nameof(SecurityController) }.{ nameof(ConfirmTwoFaPin) }: invalid PIN, logging out.");
                await RemoveAuthenticationFor(account.Id);
                
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed });
            }
            
            await _redisCache.InsertRedisCacheEntry(new CacheEntry { EntryKey = $"{ SharedConstants.TwoFaCacheName }_{ accountId }", Data = true });
            return new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }

        /// <summary>
        /// For both. To send the Two-Factor PIN to user's phone number and email address.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     GET /security/send-tfa-pin
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
        [HttpGet("send-tfa-pin")]
        [MainAuthorize]
        public async Task<JsonResult> SendTwoFaPinToEmailAndSms([FromHeader] string accountId) {
            _logger.LogInformation($"{ nameof(SecurityController) }.{ nameof(SendTwoFaPinToEmailAndSms) }: service starts.");

            var account = await _accountService.GetAccountById(accountId);
            if (account is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var twoFaPin = _googleService.GetTwoFaPin(account.TwoFaSecretKey);

            var isSmsSuccess = await SendSms($"Your Two-Factor Authentication PIN: { twoFaPin }");
            var isEmailSuccess = await SendEmail(new EmailComposer {
                EmailType = EmailType.TwoFaPin,
                ReceiverEmail = account.EmailAddress,
                Subject = $"{ nameof(COSC2640A3) } - Your Two-FA PIN",
                Contents = new Dictionary<string, string> {
                    { "TFA_CODE_PLACEHOLDER", twoFaPin }
                }
            });

            if ((!isSmsSuccess.HasValue && !isEmailSuccess.HasValue) || (isSmsSuccess.HasValue && !isSmsSuccess.Value) || (isEmailSuccess.HasValue && !isEmailSuccess.Value))
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "Failed to send PIN to your phone number and email." } });
            
            return new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }

        /// <summary>
        /// For both. To verify token sent to user's phone number to confirm the phone number.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     GET /security/verify-sms-token/{string}
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        /// </code>
        /// -->
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="smsToken">The SMS token to verify.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [HttpGet("verify-sms-token/{smsToken}")]
        [MainAuthorize]
        public async Task<JsonResult> VerifySmsTokenForPhoneNumberConfirmation([FromHeader] string accountId,[FromRoute] string smsToken) {
            _logger.LogInformation($"{ nameof(SecurityController) }.{ nameof(VerifySmsTokenForPhoneNumberConfirmation) }: service starts.");
            
            var account = await _accountService.GetAccountById(accountId);
            if (account is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            
            if (account.RecoveryToken is null || !account.TokenSetOn.HasValue)
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "There seems to be nothing to confirm." } });
            
            if (account.TokenSetOn.Value.AddHours(_tokenDuration) < DateTime.UtcNow)
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "The confirmation token has expired." } });
            
            if (!account.RecoveryToken.Equals(smsToken))
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "The confirmation token is invalid." } });

            account.PhoneNumberConfirmed = true;
            account.RecoveryToken = null;
            account.TokenSetOn = null;

            var updateAccountResult = await _accountService.UpdateAccount(account);
            return !updateAccountResult.HasValue || !updateAccountResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }

        /// <summary>
        /// For both. To request a new SMS token: send the token to user's phone number.
        /// This service is necessary when the old SMS token is expired.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// <!--
        /// <code>
        ///     GET /security/request-sms-token/{string}
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        /// </code>
        /// -->
        /// </remarks>
        /// <param name="accountId" type="string">The account's ID.</param>
        /// <param name="recaptchaToken">The token obtained from Google Recaptcha.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string] }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [HttpGet("request-sms-token/{recaptchaToken}")]
        [MainAuthorize]
        [TwoFaAuthorize]
        public async Task<JsonResult> RequestNewSmsTokenForPhoneNumberConfirmation([FromHeader] string accountId,[FromRoute] string recaptchaToken) {
            _logger.LogInformation($"{ nameof(SecurityController) }.{ nameof(RequestNewSmsTokenForPhoneNumberConfirmation) }: service starts.");
            
            var account = await _accountService.GetAccountById(accountId);
            if (account is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            return await GenerateNewAccountTokenFor(account, NotificationType.Sms);
        }

        /// <summary>
        /// For guest. To request a new account recovery token: send the token to user's phone number and email address.
        /// This service is necessary when user forgets their account's password.
        /// </summary>
        /// <remarks>
        /// Request signature:
        /// - Only provide `<c>username</c>` or `<c>email</c>` at once
        /// - <c>recaptchaToken</c> is not required for testings
        /// - <c></c>
        /// <!--
        /// <code>
        ///     POST /security/request-recovery-token
        ///     Headers
        ///         "AccountId": string
        ///         "Authorization": "Bearer token"
        ///     Body
        ///         {
        ///             email: string,
        ///             username: string,
        ///             recaptchaToken: string
        ///         }
        /// </code>
        /// -->
        /// </remarks>
        /// <param name="identity">The detail of forgotten account to send recovery data to.</param>
        /// <returns>JsonResponse object: { Result = 0|1, Messages = [string], Data = string }</returns>
        /// <response code="200">The request was successfully processed.</response>
        /// <response code="401">Authorization failed: expired or mismatched or insufficient.</response>
        [HttpPost("request-recovery-token")]
        public async Task<JsonResult> RequestNewTokenForAccountRecovery([FromBody] Identity identity) {
            _logger.LogInformation($"{ nameof(SecurityController) }.{ nameof(RequestNewTokenForAccountRecovery) }: service starts.");
            
            var isHuman = await _googleService.IsHumanInteraction(identity.RecaptchaToken);
            if (!isHuman.Result) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "Recaptcha verification failed." } });

            var account = await _accountService.GetAccountByEmailOrUsername(identity.Email, identity.Username, null);
            if (account is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var (result, message) = await _authenticationService.SendForgotPasswordToCognito(account);
            if (!result) return message is null
                ? new JsonResult(new JsonResponse {Result = RequestResult.Failed, Messages = new[] {"An issue happened while processing your request."}})
                : new JsonResult(new JsonResponse {Result = RequestResult.Failed, Messages = new[] {$"{message}"}});
            
            _ = await SendEmail(new EmailComposer {
                EmailType = EmailType.PasswordRecovery,
                ReceiverEmail = account.EmailAddress,
                Subject = $"{ nameof(COSC2640A3) } - Reset Password",
                Contents = new Dictionary<string, string> {
                    { "USER_NAME_PLACEHOLDER", account.Username },
                    { "WEBSITE_URL_PLACEHOLDER", SharedConstants.ClientUrl },
                    { "ACCOUNT_ID_PLACEHOLDER", account.Id }
                }
            });
                
            return new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = account.Id });
        }
    }
}