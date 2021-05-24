using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.Translate;
using Amazon.Translate.Model;
using AmazonLibrary.Contexts;
using AmazonLibrary.Interfaces;
using Helper.Shared;
using Microsoft.Extensions.Logging;

namespace AmazonLibrary.Services {

    public sealed class TranslateService : ITranslateService {

        private readonly ILogger<TranslateService> _logger;
        private readonly IAmazonTranslate _translateContext;

        public TranslateService(
            ILogger<TranslateService> logger,
            AmazonTranslateContext translateContext
        ) {
            _logger = logger;
            _translateContext = translateContext.GetInstance();
        }

        public async Task<KeyValuePair<bool, string>> Translate(SharedEnums.Language target, string phrase) {
            _logger.LogInformation($"{ nameof(TranslateService) }.{ nameof(Translate) }: Service starts.");

            var translateRequest = new TranslateTextRequest {
                Text = phrase,
                SourceLanguageCode = SharedConstants.LanguageCodes[(byte) SharedEnums.Language.English],
                TargetLanguageCode = SharedConstants.LanguageCodes[(byte) target]
            };

            try {
                var response = await _translateContext.TranslateTextAsync(translateRequest);
                if (response.HttpStatusCode != HttpStatusCode.OK) throw new InternalServerException("Failed to send request");

                return new KeyValuePair<bool, string>(true, response.TranslatedText);
            }
            catch (InternalServerException e) {
                _logger.LogError($"{ nameof(TranslateService) }.{ nameof(Translate) } - { nameof(InternalServerException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (InvalidRequestException e) {
                _logger.LogError($"{ nameof(TranslateService) }.{ nameof(Translate) } - { nameof(InvalidRequestException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (ResourceNotFoundException e) {
                _logger.LogError($"{ nameof(TranslateService) }.{ nameof(Translate) } - { nameof(ResourceNotFoundException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (ServiceUnavailableException e) {
                _logger.LogError($"{ nameof(TranslateService) }.{ nameof(Translate) } - { nameof(ServiceUnavailableException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (TooManyRequestsException e) {
                _logger.LogError($"{ nameof(TranslateService) }.{ nameof(Translate) } - { nameof(TooManyRequestsException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, default);
            }
            catch (TextSizeLimitExceededException e) {
                _logger.LogError($"{ nameof(TranslateService) }.{ nameof(Translate) } - { nameof(TextSizeLimitExceededException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, $"The { nameof(phrase) } is too long for translation.");
            }
            catch (DetectedLanguageLowConfidenceException e) {
                _logger.LogInformation($"{ nameof(TranslateService) }.{ nameof(Translate) } - { nameof(DetectedLanguageLowConfidenceException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, $"Unable to determine source language. Please select a source language and try again.");
            }
            catch (UnsupportedLanguagePairException e) {
                _logger.LogInformation($"{ nameof(TranslateService) }.{ nameof(Translate) } - { nameof(UnsupportedLanguagePairException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool, string>(default, $"Our Translation has not supported the selected language pair. We are sorry for this inconvenience.");
            }
        }
    }
}