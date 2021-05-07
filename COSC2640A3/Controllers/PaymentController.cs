using System;
using System.Threading.Tasks;
using AssistantLibrary.Interfaces;
using COSC2640A3.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace COSC2640A3.Controllers {

    [ApiController]
    [MainAuthorize]
    [TwoFaAuthorize]
    [Route("payment")]
    public sealed class PaymentController {

        private readonly ILogger<PaymentController> _logger;
        private readonly IPaypalService _paypalService;
        private readonly IStripeService _stripeService;

        public PaymentController(
            Logger<PaymentController> logger,
            IPaypalService paypalService,
            IStripeService stripeService
        ) {
            _logger = logger;
            _paypalService = paypalService;
            _stripeService = stripeService;
        }

        [HttpPost("paypal")]
        public async Task<JsonResult> CapturePaypalPayment() {
            _logger.LogInformation($"{ nameof(PaymentController) }.{ nameof(CapturePaypalPayment) }: Service starts.");
            throw new NotImplementedException();
        }

        [HttpPost("stripe")]
        public async Task<JsonResult> CaptureStripeCardPayment() {
            _logger.LogInformation($"{ nameof(PaymentController) }.{ nameof(CapturePaypalPayment) }: Service starts.");
            throw new NotImplementedException();
        }
    }
}