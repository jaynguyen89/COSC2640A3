using System.Threading.Tasks;
using COSC2640A3.Attributes;
using COSC2640A3.Bindings;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.ViewModels;
using Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using static Helper.Shared.SharedEnums;

namespace COSC2640A3.Controllers {

    [ApiController]
    [Route("authentication")]
    public sealed class AuthenticationController : ControllerBase {

        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAccountService _accountService;
        private readonly IRedisCacheService _redisCache;

        public AuthenticationController(
            ILogger<AuthenticationController> logger,
            IAuthenticationService authenticationService,
            IAccountService accountService,
            IRedisCacheService redisCache
        ) {
            _logger = logger;
            _authenticationService = authenticationService;
            _accountService = accountService;
            _redisCache = redisCache;
        }

        [HttpPost("register")]
        public async Task<JsonResult> Register(RegistrationVM registration) {
            _logger.LogInformation($"{ nameof(AuthenticationController) }.{ nameof(Register) }: service starts.");
            
            var errors = registration.VerifyRegistrationDetails();
            if (errors.Length != 0) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = errors });

            var (error, unavailableField) = await _accountService.IsUsernameAndEmailAvailable(registration);
            if (!error.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!error.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { $"{ unavailableField } has been used to register another account." } });

            var newAccountId = await _authenticationService.InsertToUserPool(registration);
            if (!Helpers.IsProperString(newAccountId)) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var newAccount = (Account) registration;
            newAccount.Id = newAccountId;

            var saveResult = await _accountService.InsertToDatabase(newAccount);
            return !saveResult.HasValue || !saveResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }

        [HttpPost("confirm-registration")]
        public async Task<JsonResult> ConfirmRegistration(ConfirmRegistrationVM confirmation) {
            _logger.LogInformation($"{ nameof(AuthenticationController) }.{ nameof(ConfirmRegistration) }: service starts.");
            
            var errors = confirmation.VerifyConfirmation();
            if (errors.Length != 0) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = errors });

            var account = await _accountService.GetAccountByEmailOrUsername(confirmation.Email, confirmation.Username, false);
            if (account is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            
            var (isConfirmed, message) = await _authenticationService.ConfirmUserInPool(account.Username, confirmation.ConfirmCode);
            if (!isConfirmed.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isConfirmed.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { message } });

            account.EmailConfirmed = true;
            isConfirmed = await _accountService.Update(account);

            return !isConfirmed.HasValue || !isConfirmed.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse {  Result = RequestResult.Success });
        }

        [HttpPost("authenticate")]
        public async Task<JsonResult> Authenticate(LoginCredentials credentials) {
            _logger.LogInformation($"{ nameof(AuthenticationController) }.{ nameof(Authenticate) }: service starts.");

            var errors = credentials.VerifyCredentials();
            if (errors.Length != 0) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = errors });
            
            var account = await _accountService.GetAccountByEmailOrUsername(credentials.Email, credentials.Username);
            if (account is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var (isSuccess, authTokenOrMessage) = await _authenticationService.Authenticate(account.Username, credentials.Password);
            if (!isSuccess.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isSuccess.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { authTokenOrMessage } });

            var authUser = new AuthenticatedUser { AuthToken = authTokenOrMessage, AccountId = account.Id };

            await _redisCache.InsertRedisCacheEntry(new CacheEntry { EntryKey = nameof(AuthenticatedUser.AuthToken), Data = authUser.AuthToken });
            await _redisCache.InsertRedisCacheEntry(new CacheEntry { EntryKey = nameof(AuthenticatedUser.AccountId), Data = authUser.AccountId });
            
            HttpContext.Response.Cookies.Append(nameof(AuthenticatedUser.AuthToken), authTokenOrMessage);
            return new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = authUser });
        }

        [MainAuthorize]
        [HttpGet("unauthenticate")]
        public async Task<JsonResult> Unauthenticate() {
            await _redisCache.RemoveCacheEntry(nameof(AuthenticatedUser.AuthToken));
            await _redisCache.RemoveCacheEntry(nameof(AuthenticatedUser.AccountId));
            
            HttpContext.Response.Cookies.Delete("AuthToken");
            return new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }
    }
}