using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using AmazonLibrary.Interfaces;
using AmazonLibrary.Models;
using Helper;
using Helper.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Helper.Shared.SharedEnums;

namespace AmazonLibrary.Services {

    public sealed class AmazonMailService : IAmazonMailService {

        private readonly ILogger<AmazonMailService> _logger;
        private readonly AmazonSimpleEmailServiceClient _emailService;

        private readonly string FromAddress;

        public AmazonMailService(
            ILogger<AmazonMailService> logger,
            IOptions<AmazonOptions> options
        ) {
            _logger = logger;
            FromAddress = options.Value.MailSentFromAddress;

            _emailService = new AmazonSimpleEmailServiceClient(
                "AKIAJSENDXCAPZWGB6HQ", "HeGULGolRgnxwKIIm4K2d8E+sAoHVBukvR+5umU3",
                RegionEndpoint.GetBySystemName(options.Value.RegionEndpoint)
            );
        }

        public async Task<bool?> SendEmailSingle(EmailComposer emailComposer) {
            _logger.LogInformation($"{ nameof(AmazonMailService) }.{ nameof(SendEmailSingle) }: service starts.");

            var emailContent = GetEmailTemplateAccordingTo(emailComposer.EmailType);
            if (!Helpers.IsProperString(emailContent)) return default;

            foreach (var (placeholder, content) in emailComposer.Contents)
                emailContent = emailContent.Replace($"[{ placeholder }]", content);

            var sendEmailRequest = new SendEmailRequest {
                Source = FromAddress,
                Destination = new Destination {
                    ToAddresses = new List<string> { emailComposer.ReceiverEmail }
                },
                Message = new Message {
                    Subject = new Content(emailComposer.Subject),
                    Body = new Body {
                        Html = new Content {
                            Charset = "UTF-8",
                            Data = emailContent
                        }
                    }
                }
            };

            try {
                var response = await _emailService.SendEmailAsync(sendEmailRequest);
                return response.HttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception e) {
                _logger.LogError($"{ nameof(AmazonMailService) }.{ nameof(SendEmailSingle) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        private string GetEmailTemplateAccordingTo(EmailType emailType) {
            return emailType switch {
                EmailType.AccountActivationConfirmation => $"{ SharedConstants.EmailTemplateFolderPath }AccountActivationConfirmationEmail.html",
                EmailType.PasswordRecovery => $"{ SharedConstants.EmailTemplateFolderPath }RecoverPasswordEmail.html",
                EmailType.TwoFaPin => $"{ SharedConstants.EmailTemplateFolderPath }TwoFaCodeEmail.html",
                EmailType.TwoFaDisabledNotification => $"{ SharedConstants.EmailTemplateFolderPath }TwoFaDisabledNotificationEmail.html",
                _ => default
            };
        }
    }
}