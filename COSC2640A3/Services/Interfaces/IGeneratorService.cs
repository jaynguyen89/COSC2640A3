using System.Threading.Tasks;
using COSC2640A3.Models;

namespace COSC2640A3.Services.Interfaces {

    public interface IGeneratorService {
        
        Task<string[]> InsertMultipleAccounts(Account[] accounts);
        
        Task<bool?> InsertMultipleRoles(AccountRole[] accountRoles);
        
        Task<string[]> InsertMultipleStudents(Student[] students);
        
        Task<string[]> InsertMultipleTeachers(Teacher[] teachers);
        
        Task<bool?> InsertMultipleEnrolments(Enrolment[] enrolments);
        
        Task<string[]> InsertMultipleClassrooms(Classroom[] classrooms);
    }
}