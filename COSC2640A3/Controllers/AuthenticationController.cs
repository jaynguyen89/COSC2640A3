using System.Threading.Tasks;
using AssistantLibrary.Interfaces;
using COSC2640A3.Attributes;
using COSC2640A3.Bindings;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.ViewModels;
using Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Helper.Shared.SharedEnums;

namespace COSC2640A3.Controllers {

    [ApiController]
    [Route("authentication")]
    public sealed class AuthenticationController : ControllerBase {

        private readonly ILogger<AuthenticationController> _logger;
        private readonly IContextService _contextService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAccountService _accountService;
        private readonly IRoleService _roleService;
        private readonly IRedisCacheService _redisCache;
        private readonly IGoogleService _googleService;

        public AuthenticationController(
            ILogger<AuthenticationController> logger,
            IContextService contextService,
            IAuthenticationService authenticationService,
            IAccountService accountService,
            IRoleService roleService,
            IRedisCacheService redisCache,
            IGoogleService googleService
        ) {
            _logger = logger;
            _contextService = contextService;
            _authenticationService = authenticationService;
            _accountService = accountService;
            _roleService = roleService;
            _redisCache = redisCache;
            _googleService = googleService;
        }

        [HttpPost("register")]
        public async Task<JsonResult> Register(Registration registration) {
            _logger.LogInformation($"{ nameof(AuthenticationController) }.{ nameof(Register) }: service starts.");

            var isHuman = await _googleService.IsHumanRegistration(registration.RecaptchaToken);
            if (!isHuman.Result) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "Recaptcha verification failed." } });
            
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

        [HttpPut("confirm-registration")]
        public async Task<JsonResult> ConfirmRegistration(ConfirmRegistration confirmation) {
            _logger.LogInformation($"{ nameof(AuthenticationController) }.{ nameof(ConfirmRegistration) }: service starts.");
            
            var isHuman = await _googleService.IsHumanRegistration(confirmation.RecaptchaToken);
            if (!isHuman.Result) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "Recaptcha verification failed." } });
            
            var errors = confirmation.VerifyConfirmation();
            if (errors.Length != 0) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = errors });

            var account = await _accountService.GetAccountByEmailOrUsername(confirmation.Email, confirmation.Username, false);
            if (account is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var (isConfirmed, message) = await _authenticationService.ConfirmUserInPool(account.Username, confirmation.ConfirmCode);
            if (!isConfirmed.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isConfirmed.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { message } });

            account.EmailConfirmed = true;
            await _contextService.StartTransaction();
            
            isConfirmed = await _accountService.UpdateAccount(account);
            if (!isConfirmed.HasValue || !isConfirmed.Value) {
                await _contextService.RevertTransaction();
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            }

            var isSuccess = await _roleService.CreateRolesForAccountById(account.Id);
            if (!isSuccess.HasValue || !isSuccess.Value) {
                await _contextService.RevertTransaction();
                return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            }

            await _contextService.ConfirmTransaction();
            return new JsonResult(new JsonResponse {  Result = RequestResult.Success });
        }

        [HttpPost("authenticate")]
        public async Task<JsonResult> Authenticate(LoginCredentials credentials) {
            _logger.LogInformation($"{ nameof(AuthenticationController) }.{ nameof(Authenticate) }: service starts.");
            
            var isHuman = await _googleService.IsHumanRegistration(credentials.RecaptchaToken);
            if (!isHuman.Result) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "Recaptcha verification failed." } });

            var errors = credentials.VerifyCredentials();
            if (errors.Length != 0) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = errors });
            
            var account = await _accountService.GetAccountByEmailOrUsername(credentials.Email, credentials.Username);
            if (account is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var (isSuccess, authTokenOrMessage) = await _authenticationService.Authenticate(account.Username, credentials.Password);
            if (!isSuccess.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isSuccess.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { authTokenOrMessage } });

            var authenticatedUser = new AuthenticatedUser {
                AuthToken = authTokenOrMessage,
                AccountId = account.Id,
                Role = Role.Student
            };

            await _redisCache.InsertRedisCacheEntry(new CacheEntry { EntryKey = $"{ nameof(AuthenticatedUser) }_{ authenticatedUser.AccountId }", Data = authenticatedUser });
            
            HttpContext.Response.Cookies.Append(nameof(AuthenticatedUser.AuthToken), authTokenOrMessage);
            return new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = authenticatedUser });
        }

        [MainAuthorize]
        [HttpGet("unauthenticate")]
        public async Task<JsonResult> Unauthenticate([FromHeader] string accountId) {
            _logger.LogInformation($"{ nameof(AuthenticationController) }.{ nameof(Unauthenticate) }: service starts.");
            await _redisCache.RemoveCacheEntry($"{ nameof(AuthenticatedUser) }_{ accountId }");
            
            HttpContext.Response.Cookies.Delete("AuthToken");
            return new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }

        [MainAuthorize]
        [HttpGet("switch-role")]
        public async Task<JsonResult> SwitchRoleForAuthenticatedUser([FromHeader] string accountId) {
            _logger.LogInformation($"{ nameof(AuthenticationController) }.{ nameof(SwitchRoleForAuthenticatedUser) }: service starts.");
            
            var authenticatedUser = await _redisCache.GetRedisCacheEntry<AuthenticatedUser>($"{ nameof(AuthenticatedUser) }_{ accountId }");
            if (authenticatedUser is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            authenticatedUser.Role = authenticatedUser.Role == Role.Student ? Role.Teacher : Role.Student;
            await _redisCache.InsertRedisCacheEntry(new CacheEntry { EntryKey = $"{ nameof(AuthenticatedUser) }_{ accountId }", Data = authenticatedUser });

            return new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = authenticatedUser.Role });
        }
    }
}