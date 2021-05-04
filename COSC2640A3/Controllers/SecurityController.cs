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
using static Helper.Shared.SharedEnums;

namespace COSC2640A3.Controllers {

    [ApiController]
    [MainAuthorize]
    [Route("security")]
    public class SecurityController : ControllerBase {

        private readonly ILogger<SecurityController> _logger;
        private readonly IGoogleService _googleService;
        private readonly IAccountService _accountService;
        private readonly IRedisCacheService _redisCache;
        private readonly ISmsService _smsService;
        private readonly IAmazonMailService _mailService;

        public SecurityController(
            ILogger<SecurityController> logger,
            IGoogleService googleService,
            IAccountService accountService,
            IRedisCacheService redisCache,
            ISmsService smsService,
            IAmazonMailService mailService
        ) {
            _logger = logger;
            _googleService = googleService;
            _accountService = accountService;
            _redisCache = redisCache;
            _smsService = smsService;
            _mailService = mailService;
        }

        [HttpGet("confirm-tfa-pin/{tfaPin}")]
        public async Task<JsonResult> ConfirmTwoFaPin([FromHeader] string accountId,[FromRoute] string tfaPin) {
            _logger.LogInformation($"{ nameof(SecurityController) }.{ nameof(ConfirmTwoFaPin) }: service starts.");

            var account = await _accountService.GetAccountById(accountId);
            if (account is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            
            var isTfaValid = _googleService.VerifyTwoFactorAuth(account.TwoFaSecretKey, tfaPin);
            if (!isTfaValid) {
                _logger.LogInformation($"{ nameof(SecurityController) }.{ nameof(ConfirmTwoFaPin) }: invalid PIN, logging out.");
                
                await _redisCache.RemoveCacheEntry($"{ nameof(AuthenticatedUser) }_{ accountId }");
                await _redisCache.RemoveCacheEntry($"{ SharedConstants.TwoFaCacheName }_{ accountId }");
            
                HttpContext.Response.Cookies.Delete("AuthToken");
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed });
            }
            
            await _redisCache.InsertRedisCacheEntry(new CacheEntry { EntryKey = $"{ nameof(AuthenticatedUser) }_{ SharedConstants.TwoFaCacheName }", Data = true });
            return new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }

        [HttpGet("send-tfa-pin")]
        public async Task<JsonResult> SendTwoFaPinToEmailAndSms([FromHeader] string accountId) {
            _logger.LogInformation($"{ nameof(SecurityController) }.{ nameof(SendTwoFaPinToEmailAndSms) }: service starts.");

            var account = await _accountService.GetAccountById(accountId);
            if (account is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var twoFaPin = _googleService.GetTwoFaPin(account.TwoFaSecretKey);

            var isSmsSuccess = await _smsService.SendSmsWithContent(twoFaPin);
            var isEmailSuccess = await _mailService.SendEmailSingle(new EmailComposer {
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
    }
}