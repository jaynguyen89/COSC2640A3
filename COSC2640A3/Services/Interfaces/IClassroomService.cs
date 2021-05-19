using System.Threading.Tasks;
using COSC2640A3.Models;
using COSC2640A3.ViewModels;
using COSC2640A3.ViewModels.Exports;
using COSC2640A3.ViewModels.Features;

namespace COSC2640A3.Services.Interfaces {

    public interface IClassroomService {

        /// <summary>
        /// Returns default if error, otherwise returns the ID of new classroom.
        /// </summary>
        Task<string> InsertNewClassroom(Classroom classroom);
        
        /// <summary>
        /// Returns null if error, returns false if failed, returns true if success.
        /// </summary>
        Task<bool?> UpdateClassroom(Classroom classroom);
        
        /// <summary>
        /// Returns null if error, returns false if not belonged, returns true if belonged.
        /// </summary>
        Task<bool?> IsClassroomBelongedToThisTeacherByAccountId(string accountId, string classroomId);
        
        Task<Classroom> GetClassroomById(string classroomId);
        
        /// <summary>
        /// Returns null if error, returns false if failed, returns true if success.
        /// </summary>
        Task<bool?> DeleteClassroom(Classroom classroom);
        
        Task<ClassroomVM[]> GetAllClassroomsByTeacherId(string teacherId, bool isActive = true);
        
        Task<string[]> GetIdsOfClassroomsAlreadyEnrolledByStudentWith(string accountId);
        
        Task<ClassroomVM[]> GetAllClassroomsExcludingFrom(string teacherId, string[] enrolledClassroomIds);
        
        Task<ClassroomVM> GetClassroomDetailsFor(string classroomId);
        
        Task<EnrolmentVM[]> GetStudentEnrolmentsByClassroomId(string classroomId);
        
        Task<Enrolment[]> GetEnrolmentsByClassroomId(string classroomId);
        
        Task<bool?> AreTheseClassroomsBelongedTo(string accountId);
        
        Task<ClassroomExportVM[]> GetClassroomDataForExportBy(string[] classroomIds);
    }
}