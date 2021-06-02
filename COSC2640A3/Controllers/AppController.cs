using System;
using System.Threading.Tasks;
using AmazonLibrary.Interfaces;
using AmazonLibrary.Models;
using AssistantLibrary.Interfaces;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.ViewModels;
using Helper;
using Helper.Shared;
using Microsoft.AspNetCore.Mvc;
using static Helper.Shared.SharedEnums;

namespace COSC2640A3.Controllers {

    public class AppController : ControllerBase {

        protected readonly IContextService _contextService;
        protected readonly IAccountService _accountService;
        protected readonly IRedisCacheService _redisCache;
        private readonly ISmsService _smsService;
        private readonly IAmazonMailService _mailService;

        protected AppController() { }

        protected AppController(
            IContextService contextService,
            IAccountService accountService,
            IRedisCacheService redisCache,
            ISmsService smsService,
            IAmazonMailService mailService
        ) {
            _contextService = contextService;
            _smsService = smsService;
            _redisCache = redisCache;
            _mailService = mailService;
            _accountService = accountService;
        }

        /// <summary>
        /// Generates a random token and saves to Account.
        /// <para>If <c>notificationType == NotificationType.Sms</c>, then <c>emailContent</c> can be `<c>null</c>` as it will be ignored.</para>
        /// <para>If <c>notificationType == NotificationType.Email | NotificationType.Both</c>, then <c>emailContent</c> <b>MUST</b> not be `<c>null</c>`.</para>
        /// </summary>
        protected async Task<JsonResult> GenerateNewAccountTokenFor(Account account, NotificationType notificationType, EmailComposer emailContent = null) {
            if (notificationType != NotificationType.Sms && emailContent is null) throw new ArgumentNullException();
            
            var confirmationToken = Helpers.GenerateRandomString();
            account.RecoveryToken = confirmationToken;
            account.TokenSetOn = DateTime.UtcNow;
            
            await _contextService.StartTransaction();
            var updateAccountResult = await _accountService.UpdateAccount(account);
            if (!updateAccountResult.HasValue || !updateAccountResult.Value) {
                await _contextService.RevertTransaction();
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            }

            if (notificationType != NotificationType.Sms)
                emailContent?.Contents.Add("CONFIRMATION_TOKEN_PLACEHOLDER", confirmationToken);

            var notificationExpression = notificationType switch {
                NotificationType.Sms => (Func<Task<bool?>>)(async () => await SendSms($"Your confirmation code: { confirmationToken }")),
                NotificationType.Email => async () => await SendEmail(emailContent),
                NotificationType.Both => async () => {
                    var smsResult = await SendSms($"Your confirmation code: {confirmationToken}");
                    var emailResult = await SendEmail(emailContent);

                    return (smsResult.HasValue || emailResult.HasValue) && (!smsResult.HasValue || !emailResult.HasValue || smsResult.Value || emailResult.Value);
                },
                _ => () => default
            };

            var notificationResult = notificationExpression.Invoke().Result;
            if (!notificationResult.HasValue || !notificationResult.Value) {
                await _contextService.RevertTransaction();
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "Failed to send confirmation token. Please try again." } });
            }

            await _contextService.ConfirmTransaction();
            return new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }

        protected async Task<bool?> SendSms(string message) {
            return await _smsService.SendSmsWithContent(message);
        }

        protected async Task<bool?> SendEmail(EmailComposer emailComposer) {
            return await _mailService.SendEmailSingle(emailComposer);
        }

        protected async Task RemoveAuthenticationFor(string accountId) {
            await _redisCache.RemoveCacheEntry($"{ nameof(AuthenticatedUser) }_{ accountId }");
            await _redisCache.RemoveCacheEntry($"{ SharedConstants.TwoFaCacheName }_{ accountId }");
            
            HttpContext.Response.Cookies.Delete("AuthToken");
        }
        
        protected string GetBucketNameForFileType(string classroomId, byte fileType) {
            return fileType switch {
                (byte) FileType.video => $"{ classroomId }.{ nameof(FileType.video) }s".ToLower(),
                (byte) FileType.audio => $"{ classroomId }.{ nameof(FileType.audio) }s".ToLower(),
                (byte) FileType.image => $"{ classroomId }.{ nameof(FileType.image) }s".ToLower(),
                (byte) FileType.other => $"{ classroomId }.{ nameof(FileType.other) }s".ToLower(),
                _ => default
            };
        }
    }
}