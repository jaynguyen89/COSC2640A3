using System;
using COSC2640A3.Models;

namespace COSC2640A3.ViewModels.Features {

    public sealed class InvoiceVM {

        public string Id { get; set; }

        public decimal Amount { get; set; }

        public bool IsPaid { get; set; }

        public PaymentDetailVM PaymentDetail { get; set; }

        public static implicit operator InvoiceVM(Invoice invoice) {
            return new() {
                Id = invoice.Id,
                Amount = invoice.DueAmount,
                IsPaid = invoice.IsPaid
            };
        }

        public sealed class PaymentDetailVM {

            public string PaymentMethod { get; set; }

            public string PaymentId { get; set; }

            public string TransactionId { get; set; }

            public string ChargeId { get; set; }

            public string PaymentStatus { get; set; }

            public DateTime? PaidOn { get; set; }

            public static implicit operator PaymentDetailVM(Invoice invoice) {
                return new() {
                    PaymentMethod = invoice.PaymentMethod,
                    PaymentId = invoice.PaymentId,
                    TransactionId = invoice.TransactionId,
                    ChargeId = invoice.ChargeId,
                    PaymentStatus = invoice.PaymentStatus,
                    PaidOn = invoice.PaidOn
                };
            }
        }
    }
}