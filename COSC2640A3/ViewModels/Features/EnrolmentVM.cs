using System;
using COSC2640A3.Models;

namespace COSC2640A3.ViewModels.Features {

    public sealed class EnrolmentVM {
        
        public string Id { get; set; }
        
        public ClassroomVM Classroom { get; set; }
        
        public InvoiceVM Invoice { get; set; }
        
        public DateTime EnrolledOn { get; set; }

        public static implicit operator EnrolmentVM(Enrolment enrolment) {
            return new() {
                Id = enrolment.Id,
                EnrolledOn = enrolment.EnrolledOn,
                Classroom = enrolment.Classroom,
                Invoice = enrolment.Invoice
            };
        }
    }
}