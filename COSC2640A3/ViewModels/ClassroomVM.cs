using System;
using COSC2640A3.Models;
using Helper.Shared;

namespace COSC2640A3.ViewModels {

    public sealed class ClassroomVM {
        
        public string Id { get; set; }
        
        public string TeacherId { get; set; }
        
        public string TeacherName { get; set; }
        
        public string ClassName { get; set; }
        
        public short Capacity { get; set; }
        
        public decimal Price { get; set; }
        
        public DateTime? CommencedOn { get; set; }
        
        public byte Duration { get; set; }
        
        public byte DurationUnit { get; set; }
        
        public bool IsActive { get; set; }
        
        public DateTime CreatedOn { get; set; }

        public string NormalizedDuration => $"{ Duration }{ ((SharedEnums.DurationUnit) DurationUnit).ToString() }";

        public static implicit operator ClassroomVM(Classroom classroom) {
            return new() {
                Id = classroom.Id,
                TeacherId = classroom.TeacherId,
                TeacherName = classroom.Teacher.Account.PreferredName,
                ClassName = classroom.ClassName,
                Capacity = classroom.Capacity,
                Price = classroom.Price,
                CommencedOn = classroom.CommencedOn,
                Duration = classroom.Duration,
                DurationUnit = classroom.DurationUnit,
                IsActive = classroom.IsActive,
                CreatedOn = classroom.CreatedOn,
                //NormalizedDuration = $"{ classroom.Duration }{ ((SharedEnums.DurationUnit) classroom.DurationUnit).ToString() }"
            };
        }
    }
}