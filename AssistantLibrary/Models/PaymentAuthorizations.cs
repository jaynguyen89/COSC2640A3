namespace AssistantLibrary.Models {

    public class PaypalAuthorization {
        
        public string PaypalEmail { get; set; }
        
        public string OrderId { get; set; }
        
        public decimal Amount { get; set; }
        
        public string AuthorizationId { get; set; }
    }

    public sealed class StripeAuthorization {
        
        public string CardType { get; set; }
        
        public string Last4Digits { get; set; }
        
        public string TokenId { get; set; }
        
        public PaymentDetail Details { get; set; }
        
        public sealed class PaymentDetail {
            
            public string ClassroomId { get; set; }
            
            public string ClassName { get; set; }
            
            public decimal Amount { get; set; }
        }
    }
}