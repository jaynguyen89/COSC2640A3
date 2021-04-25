using System.Threading.Tasks;
using COSC2640A3.Models;
using COSC2640A3.ViewModels;

namespace COSC2640A3.Services.Interfaces {

    public interface IClassroomService {

        Task<string> InsertNewClassroom(Classroom classroom);
        
        Task<bool?> UpdateClassroom(Classroom classroom);
        
        Task<bool?> IsClassroomBelongedToThisTeacherByAccountId(string accountId, string classroomId);
        
        Task<Classroom> GetClassroomById(string classroomId);
        
        Task<bool?> DeleteClassroom(Classroom classroom);
        
        Task<ClassroomVM[]> GetAllClassroomsByTeacherId(string teacherId);
        
        Task<ClassroomVM[]> GetAllClassrooms();
    }
}