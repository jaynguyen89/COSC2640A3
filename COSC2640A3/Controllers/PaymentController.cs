﻿using System;
using System.Threading.Tasks;
using AssistantLibrary.Interfaces;
using AssistantLibrary.Models;
using COSC2640A3.Attributes;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.ViewModels;
using Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Helper.Shared.SharedEnums;

namespace COSC2640A3.Controllers {

    [ApiController]
    [MainAuthorize]
    [TwoFaAuthorize]
    [RoleAuthorize(Role.Student)]
    [Route("payment")]
    public sealed class PaymentController {

        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymentService _paymentService;
        private readonly IInvoiceService _invoiceService;

        public PaymentController(
            ILogger<PaymentController> logger,
            IPaymentService paymentService,
            IInvoiceService invoiceService
        ) {
            _logger = logger;
            _paymentService = paymentService;
            _invoiceService = invoiceService;
        }

        [HttpPost("paypal/{enrolmentId}")]
        public async Task<JsonResult> CapturePaypalPayment([FromRoute] string enrolmentId,[FromBody] PaypalAuthorization paymentAuthorization) {
            _logger.LogInformation($"{ nameof(PaymentController) }.{ nameof(CapturePaypalPayment) }: Service starts.");

            var isPaymentAuthorizationValid = await _paymentService.IsPaymentAuthorizationValid(paymentAuthorization);
            if (!isPaymentAuthorizationValid.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isPaymentAuthorizationValid.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "Your payment authorization is invalid." } });

            var isCaptureSuccess = await _paymentService.CaptureMoneyFromAuthorizedPayment(paymentAuthorization);
            if (!isCaptureSuccess.HasValue) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            if (!isCaptureSuccess.Value) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "Failed to charge your Paypal. Please try again." } });

            var invoice = await _invoiceService.GetInvoiceByEnrolmentId(enrolmentId);
            if (invoice is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            invoice.IsPaid = true;
            invoice.PaymentMethod = $"PAYPAL { paymentAuthorization.PaypalEmail.HideEmailPartial() }";
            invoice.PaymentId = paymentAuthorization.OrderId;
            invoice.TransactionId = paymentAuthorization.AuthorizationId[^10..];
            invoice.PaymentStatus = "Completed";
            invoice.PaidOn = DateTime.UtcNow;

            var updateResult = await _invoiceService.UpdateInvoice(invoice);
            return !updateResult.HasValue || !updateResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = invoice });
        }

        [HttpPost("card/{enrolmentId}")]
        public async Task<JsonResult> CaptureStripeCardPayment([FromRoute] string enrolmentId,[FromBody] StripeAuthorization paymentAuthorization) {
            _logger.LogInformation($"{ nameof(PaymentController) }.{ nameof(CaptureStripeCardPayment) }: Service starts.");

            var (transactionId, chargeId) = await _paymentService.CaptureStripePaymentFrom(paymentAuthorization);
            if (transactionId is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            var invoice = await _invoiceService.GetInvoiceByEnrolmentId(enrolmentId);
            if (invoice is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });

            invoice.IsPaid = true;
            invoice.PaymentMethod = $"{ paymentAuthorization.CardType } ***{ paymentAuthorization.Last4Digits }";
            invoice.PaymentId = Helpers.GenerateRandomString();
            invoice.TransactionId = transactionId;
            invoice.ChargeId = chargeId;
            invoice.PaymentStatus = "Completed";
            invoice.PaidOn = DateTime.UtcNow;

            var updateResult = await _invoiceService.UpdateInvoice(invoice);
            return !updateResult.HasValue || !updateResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = invoice });
        }
        
        [HttpPost("google/{enrolmentId}")]
        public async Task<JsonResult> CaptureStripeGooglePayment([FromRoute] string enrolmentId,[FromBody] StripeAuthorization paymentAuthorization) {
            _logger.LogInformation($"{ nameof(PaymentController) }.{ nameof(CaptureStripeGooglePayment) }: Service starts.");
            
            var (transactionId, chargeId) = await _paymentService.CaptureStripePaymentFrom(paymentAuthorization);
            if (transactionId is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            
            var invoice = await _invoiceService.GetInvoiceByEnrolmentId(enrolmentId);
            if (invoice is null) return new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } });
            
            invoice.IsPaid = true;
            invoice.PaymentMethod = $"{ paymentAuthorization.CardType } ***{ paymentAuthorization.Last4Digits }";
            invoice.PaymentId = Helpers.GenerateRandomString();
            invoice.TransactionId = transactionId;
            invoice.ChargeId = chargeId;
            invoice.PaymentStatus = "Completed";
            invoice.PaidOn = DateTime.UtcNow;
            
            var updateResult = await _invoiceService.UpdateInvoice(invoice);
            return !updateResult.HasValue || !updateResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success, Data = invoice });
        }
    }
}