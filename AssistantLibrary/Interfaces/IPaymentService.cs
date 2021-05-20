using System.Collections.Generic;
using System.Threading.Tasks;
using AssistantLibrary.Models;

namespace AssistantLibrary.Interfaces {

    public interface IPaymentService {

        Task<bool?> IsPaymentAuthorizationValid(PaypalAuthorization paymentAuthorization);
        
        Task<bool?> CaptureMoneyFromAuthorizedPayment(PaypalAuthorization paymentAuthorization);
        
        /// <summary>
        /// Key holds the transaction ID, Value holds the charge ID.
        /// </summary>
        Task<KeyValuePair<string, string>> CaptureStripePaymentFrom(StripeAuthorization paymentAuthorization);
    }
}