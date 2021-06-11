using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using COSC2640A3.Bindings;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LimitExceededException = Amazon.CognitoIdentityProvider.Model.LimitExceededException;

namespace COSC2640A3.Services.Services {

    public sealed class AuthenticationService : IAuthenticationService {

        private readonly ILogger<AuthenticationService> _logger;
        private readonly IAmazonCognitoIdentityProvider _identityProvider;

        private readonly string _clientId;
        private readonly string _userPoolId;

        public AuthenticationService(
            ILogger<AuthenticationService> logger,
            IOptions<MainOptions> options
        ) {
            _logger = logger;
            _clientId = options.Value.UserPoolAppClientId;
            _userPoolId = options.Value.CognitoUserPoolId;
            
            _identityProvider = new AmazonCognitoIdentityProviderClient(
                "AKIAJSENDXCAPZWGB6HQ", "HeGULGolRgnxwKIIm4K2d8E+sAoHVBukvR+5umU3",
                RegionEndpoint.GetBySystemName(options.Value.AwsRegion)
            );
        }

        public async Task<string> InsertToUserPool(Registration registration) {
            var signUpRequest = new SignUpRequest {
                ClientId = _clientId,
                Username = registration.Username,
                Password = registration.Password,
                UserAttributes = new List<AttributeType> {
                    new() { Name = nameof(Registration.Email).ToLower(), Value = registration.Email }
                }
            };

            _logger.LogInformation($"{ nameof(AuthenticationService) }.{ nameof(InsertToUserPool) }: Send registration data to save in AWS Cognito User Pool.");
            
            try {
                var response = await _identityProvider.SignUpAsync(signUpRequest);
                if (response.HttpStatusCode != HttpStatusCode.OK) throw new InternalErrorException("Request to AWS Cognito failed.");

                return response.UserSub;
            }
            catch (InternalErrorException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(InsertToUserPool) } - { nameof(InternalErrorException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (InvalidParameterException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(InsertToUserPool) } - { nameof(InvalidParameterException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (InvalidPasswordException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(InsertToUserPool) } - { nameof(InvalidPasswordException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (ResourceNotFoundException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(InsertToUserPool) } - { nameof(ResourceNotFoundException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (TooManyRequestsException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(InsertToUserPool) } - { nameof(TooManyRequestsException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (UsernameExistsException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(InsertToUserPool) } - { nameof(UsernameExistsException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<KeyValuePair<bool?, string>> Authenticate(string username, string password) {

            var authenticationRequest = new AdminInitiateAuthRequest {
                ClientId = _clientId,
                UserPoolId = _userPoolId,
                AuthFlow = AuthFlowType.ADMIN_USER_PASSWORD_AUTH,
                AuthParameters = new Dictionary<string, string> {
                    { "USERNAME", username },
                    { "PASSWORD", password }
                }
            };

            _logger.LogInformation($"{ nameof(AuthenticationService) }.{ nameof(Authenticate) }: Send authentication request to AWS User Pool.");
            try {
                var response = await _identityProvider.AdminInitiateAuthAsync(authenticationRequest);
                if (response.HttpStatusCode != HttpStatusCode.OK) throw new InternalErrorException("Request to AWS Cognito failed.");

                return new KeyValuePair<bool?, string>(true, response.AuthenticationResult.IdToken);
            }
            catch (InternalErrorException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(Authenticate) } - { nameof(InternalErrorException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool?, string>(default, default);
            }
            catch (InvalidParameterException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(Authenticate) } - { nameof(InvalidParameterException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool?, string>(default, default);
            }
            catch (InvalidUserPoolConfigurationException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(Authenticate) } - { nameof(InvalidUserPoolConfigurationException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool?, string>(default, default);
            }
            catch (ResourceNotFoundException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(Authenticate) } - { nameof(ResourceNotFoundException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool?, string>(default, default);
            }
            catch (TooManyRequestsException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(Authenticate) } - { nameof(TooManyRequestsException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool?, string>(default, default);
            }
            catch (NotAuthorizedException) {
                return new KeyValuePair<bool?, string>(false, "Your account do not match any entity.");
            }
            catch (UserNotConfirmedException) {
                return new KeyValuePair<bool?, string>(false, "Your account has not been confirmed.");
            }
            catch (UserNotFoundException) {
                return new KeyValuePair<bool?, string>(false, "Your account do not match any entity.");
            }
            catch (PasswordResetRequiredException) {
                return new KeyValuePair<bool?, string>(false, "You need to reset password before attempting to login.");
            }
        }

        public async Task<KeyValuePair<bool?, string>> ConfirmUserInPool(string username, string code) {
            var confirmRequest = new ConfirmSignUpRequest {
                ClientId = _clientId,
                Username = username,
                ConfirmationCode = code
            };

            _logger.LogInformation($"{ nameof(AuthenticationService) }.{ nameof(ConfirmUserInPool) }: Send confirmation request to AWS User Pool.");
            try {
                var response = await _identityProvider.ConfirmSignUpAsync(confirmRequest);
                if (response.HttpStatusCode != HttpStatusCode.OK) throw new InternalErrorException("Request to AWS Cognito failed.");

                return new KeyValuePair<bool?, string>(true, default);
            }
            catch (AliasExistsException) {
                return new KeyValuePair<bool?, string>(false, "Unable to find your account with the provided data.");
            }
            catch (CodeMismatchException) {
                return new KeyValuePair<bool?, string>(false, "Invalid confirmation code.");
            }
            catch (ExpiredCodeException) {
                return new KeyValuePair<bool?, string>(false, "Confirmation code has expired.");
            }
            catch (UserNotFoundException) {
                return new KeyValuePair<bool?, string>(false, "Unable to find your account with the provided data.");
            }
            catch (InternalErrorException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(ConfirmUserInPool) } - { nameof(InternalErrorException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool?, string>(default, default);
            }
            catch (InvalidParameterException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(ConfirmUserInPool) } - { nameof(InvalidParameterException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool?, string>(default, default);
            }
            catch (LimitExceededException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(ConfirmUserInPool) } - { nameof(LimitExceededException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool?, string>(default, default);
            }
            catch (NotAuthorizedException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(ConfirmUserInPool) } - { nameof(NotAuthorizedException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool?, string>(default, default);
            }
            catch (ResourceNotFoundException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(ConfirmUserInPool) } - { nameof(ResourceNotFoundException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool?, string>(default, default);
            }
            catch (TooManyFailedAttemptsException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(ConfirmUserInPool) } - { nameof(TooManyFailedAttemptsException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool?, string>(default, default);
            }
            catch (TooManyRequestsException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(ConfirmUserInPool) } - { nameof(TooManyRequestsException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool?, string>(default, default);
            }
        }

        public async Task<KeyValuePair<bool, string>> SendForgotPasswordToCognito(Account account) {
            var forgotPasswordRequest = new ForgotPasswordRequest {
                ClientId = _clientId,
                Username = account.Username
            };

            try {
                var response = await _identityProvider.ForgotPasswordAsync(forgotPasswordRequest);
                if (response.HttpStatusCode != HttpStatusCode.OK) throw new InternalErrorException("Request to AWS Cognito failed.");

                return new KeyValuePair<bool, string>(true, default);
            }
            catch (UserNotConfirmedException) {
                return new KeyValuePair<bool, string>(default, "Your account has not been confirmed. Unable to recover password. Please register another account.");
            }
            catch (UserNotFoundException) {
                return new KeyValuePair<bool, string>(default, "Unable to find your account with the provided data.");
            }
            catch (CodeDeliveryFailureException) {
                return new KeyValuePair<bool, string>(default, "An issue that prevents us to send recovery email to you. Please try again.");
            }
            catch (InternalErrorException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(SendForgotPasswordToCognito) } - { nameof(InternalErrorException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (InvalidEmailRoleAccessPolicyException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(SendForgotPasswordToCognito) } - { nameof(InvalidEmailRoleAccessPolicyException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (InvalidLambdaResponseException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(SendForgotPasswordToCognito) } - { nameof(InvalidLambdaResponseException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (InvalidParameterException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(SendForgotPasswordToCognito) } - { nameof(InvalidParameterException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (InvalidSmsRoleAccessPolicyException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(SendForgotPasswordToCognito) } - { nameof(InvalidSmsRoleAccessPolicyException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (InvalidSmsRoleTrustRelationshipException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(SendForgotPasswordToCognito) } - { nameof(InvalidSmsRoleTrustRelationshipException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (LimitExceededException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(SendForgotPasswordToCognito) } - { nameof(LimitExceededException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (NotAuthorizedException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(SendForgotPasswordToCognito) } - { nameof(NotAuthorizedException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (ResourceNotFoundException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(SendForgotPasswordToCognito) } - { nameof(ResourceNotFoundException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (TooManyRequestsException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(SendForgotPasswordToCognito) } - { nameof(TooManyRequestsException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (UnexpectedLambdaException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(SendForgotPasswordToCognito) } - { nameof(UnexpectedLambdaException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (UserLambdaValidationException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(SendForgotPasswordToCognito) } - { nameof(UserLambdaValidationException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
        }

        public async Task<KeyValuePair<bool, string>> ConfirmPasswordReset(PasswordReset passwordReset, string username) {
            var confirmPasswordRequest = new ConfirmForgotPasswordRequest {
                ClientId = _clientId,
                ConfirmationCode = passwordReset.RecoveryToken,
                Password = passwordReset.Password,
                Username = username
            };

            try {
                var response = await _identityProvider.ConfirmForgotPasswordAsync(confirmPasswordRequest);
                if (response.HttpStatusCode != HttpStatusCode.OK) throw new InternalErrorException("Request to AWS Cognito failed.");
                
                return new KeyValuePair<bool, string>(true, default);
            }
            catch (UserNotConfirmedException) {
                return new KeyValuePair<bool, string>(default, "Your account has not been confirmed. Unable to recover password. Please register another account.");
            }
            catch (UserNotFoundException) {
                return new KeyValuePair<bool, string>(default, "Unable to find your account with the provided data.");
            }
            catch (CodeMismatchException) {
                return new KeyValuePair<bool, string>(default, "The confirmation token does not match our record. Please try again.");
            }
            catch (ExpiredCodeException) {
                return new KeyValuePair<bool, string>(default, "The confirmation token is expired. Please attempt Forgot Password again.");
            }
            catch (InvalidPasswordException) {
                return new KeyValuePair<bool, string>(default, "The new password does not match our security constraints. Please use a stronger password.");
            }
            catch (InternalErrorException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(ConfirmPasswordReset) } - { nameof(InternalErrorException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (InvalidParameterException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(ConfirmPasswordReset) } - { nameof(InvalidParameterException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (LimitExceededException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(ConfirmPasswordReset) } - { nameof(LimitExceededException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (NotAuthorizedException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(ConfirmPasswordReset) } - { nameof(NotAuthorizedException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (ResourceNotFoundException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(ConfirmPasswordReset) } - { nameof(ResourceNotFoundException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (TooManyFailedAttemptsException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(ConfirmPasswordReset) } - { nameof(TooManyFailedAttemptsException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (TooManyRequestsException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(ConfirmPasswordReset) } - { nameof(TooManyRequestsException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (UnexpectedLambdaException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(ConfirmPasswordReset) } - { nameof(UnexpectedLambdaException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (UserLambdaValidationException e) {
                _logger.LogError($"{ nameof(AuthenticationService) }.{ nameof(ConfirmPasswordReset) } - { nameof(UserLambdaValidationException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
        }
    }
}