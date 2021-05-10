using System;
using COSC2640A3.Models;

namespace COSC2640A3.ViewModels.Exports {

    public sealed class ClassroomExportVM {
        
        public string TeacherId { get; set; }
        
        public string ClassName { get; set; }
        
        public short Capacity { get; set; }
        
        public decimal Price { get; set; }
        
        public DateTime? CommencedOn { get; set; }
        
        public byte Duration { get; set; }
        
        public byte DurationUnit { get; set; }
        
        public bool IsActive { get; set; }
        
        public DateTime CreatedOn { get; set; }

        public static implicit operator ClassroomExportVM(Classroom classroom) {
            return new() {
                TeacherId = classroom.TeacherId,
                ClassName = classroom.ClassName,
                Capacity = classroom.Capacity,
                Price = classroom.Price,
                CommencedOn = classroom.CommencedOn,
                Duration = classroom.Duration,
                DurationUnit = classroom.DurationUnit,
                IsActive = classroom.IsActive,
                CreatedOn = classroom.CreatedOn
            };
        }
    }
}