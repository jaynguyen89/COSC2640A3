using System;
using COSC2640A3.Models;

namespace COSC2640A3.ViewModels.Exports {

    public sealed class EnrolmentExportVM {
        
        public string ClassroomId { get; set; }
        
        public EnrolmentExport[] Enrolments { get; set; }
        
        public class EnrolmentExport {
                
                public string StudentId { get; set; }
                
                public string InvoiceId { get; set; }
                
                public DateTime EnrolledOn { get; set; }
                
                public byte? OverallMark { get; set; }
                
                public string MarkBreakdowns { get; set; }
                
                public bool? IsPassed { get; set; }
                
                public InvoiceExport Invoice { get; set; }

                public static implicit operator EnrolmentExport(Enrolment enrolment) {
                    return new() {
                        StudentId = enrolment.StudentId,
                        InvoiceId = enrolment.InvoiceId,
                        EnrolledOn = enrolment.EnrolledOn,
                        OverallMark = enrolment.OverallMark,
                        MarkBreakdowns = enrolment.MarkBreakdowns,
                        IsPassed = enrolment.IsPassed
                    };
                }

                public class InvoiceExport {
                    
                    public decimal DueAmount { get; set; }
                    
                    public bool IsPaid { get; set; }
                    
                    public string PaymentMethod { get; set; }
                    
                    public string PaymentId { get; set; }
                    
                    public string TransactionId { get; set; }
                    
                    public string ChargeId { get; set; }
                    
                    public string PaymentStatus { get; set; }
                    
                    public DateTime? PaidOn { get; set; }

                    public static implicit operator InvoiceExport(Invoice invoice) {
                        return new() {
                            DueAmount = invoice.DueAmount,
                            IsPaid = invoice.IsPaid,
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
}