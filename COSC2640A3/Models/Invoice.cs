using System;
using System.Collections.Generic;

#nullable disable

namespace COSC2640A3.Models
{
    public partial class Invoice
    {
        public Invoice()
        {
            Enrolments = new HashSet<Enrolment>();
        }

        public string Id { get; set; }
        public decimal DueAmount { get; set; }
        public bool IsPaid { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string ChargeId { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? PaidOn { get; set; }

        public virtual ICollection<Enrolment> Enrolments { get; set; }
    }
}
