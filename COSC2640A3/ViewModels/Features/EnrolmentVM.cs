using System;
using COSC2640A3.Models;
using COSC2640A3.ViewModels.Account;
using Helper;
using Newtonsoft.Json;

namespace COSC2640A3.ViewModels.Features {

    public sealed class EnrolmentVM {
        
        public string Id { get; set; }
        
        public StudentVM Student { get; set; } // Has value when instantiated by Teacher role, otherwise null
        
        public ClassroomVM Classroom { get; set; } // Has value when instantiated by Student role, otherwise null
        
        public InvoiceVM Invoice { get; set; }
        
        public MarksDetailVM MarksDetail { get; set; }
        
        public DateTime EnrolledOn { get; set; }

        public static implicit operator EnrolmentVM(Enrolment enrolment) {
            var enrolmentVm = new EnrolmentVM() {
                Id = enrolment.Id,
                EnrolledOn = enrolment.EnrolledOn,
                Classroom = enrolment.Classroom,
                Invoice = enrolment.Invoice
            };

            if (enrolment.OverallMark.HasValue && Helpers.IsProperString(enrolment.MarkBreakdowns))
                enrolmentVm.MarksDetail = enrolment;

            return enrolmentVm;
        }
        
        public sealed class MarksDetailVM {
            
            public byte? OverallMarks { get; set; }
            
            public MarkBreakdownVM[] MarkBreakdowns { get; set; }
            
            public bool? IsPassed { get; set; }

            public static implicit operator MarksDetailVM(Enrolment enrolment) {
                return new() {
                    OverallMarks = enrolment.OverallMark,
                    IsPassed = enrolment.IsPassed,
                    MarkBreakdowns = JsonConvert.DeserializeObject<MarkBreakdownVM[]>(enrolment.MarkBreakdowns)
                };
            }
        }
    }
}