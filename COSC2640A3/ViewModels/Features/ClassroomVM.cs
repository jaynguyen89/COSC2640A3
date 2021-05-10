using System;
using COSC2640A3.Models;
using Helper.Shared;

namespace COSC2640A3.ViewModels.Features {

    public sealed class ClassroomVM {

        public string Id { get; set; }

        public string TeacherId { get; set; }

        public string TeacherName { get; set; }

        public string ClassName { get; set; }

        public decimal Price { get; set; }

        public ClassroomDetailVM ClassroomDetail { get; set; }

        public static implicit operator ClassroomVM(Classroom classroom) {
            return classroom is null ? null : new ClassroomVM {
                Id = classroom.Id,
                TeacherId = classroom.TeacherId,
                TeacherName = classroom.Teacher is null ? string.Empty : classroom.Teacher.Account.PreferredName,
                ClassName = classroom.ClassName,
                Price = classroom.Price
            };
        }

        public void SetClassroomDetail(Classroom classroom) {
            ClassroomDetail = classroom;
        }

        public sealed class ClassroomDetailVM {

            public short Capacity { get; set; }

            public DateTime? CommencedOn { get; set; }

            public byte Duration { get; set; }

            public byte DurationUnit { get; set; }

            public bool IsActive { get; set; }

            public DateTime CreatedOn { get; set; }

            public string NormalizedDuration => $"{ Duration }{ SharedConstants.MonoSpace }{ ((SharedEnums.DurationUnit) DurationUnit).ToString() }";

            public static implicit operator ClassroomDetailVM(Classroom classroom) {
                return new() {
                    Capacity = classroom.Capacity,
                    CommencedOn = classroom.CommencedOn,
                    Duration = classroom.Duration,
                    DurationUnit = classroom.DurationUnit,
                    IsActive = classroom.IsActive,
                    CreatedOn = classroom.CreatedOn
                };
            }
        }
    }

}