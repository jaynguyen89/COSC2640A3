using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using COSC2640A3.Bindings;
using COSC2640A3.Models;

namespace COSC2640A3.Services.Interfaces {

    public interface IAccountService {
        
        /// <summary>
        /// Key == null: error, Key == true: available, Key == false: unavailable then Value == nameof(registration.Email) | nameof(registration.Username)
        /// </summary>
        Task<KeyValuePair<bool?, string>> IsUsernameAndEmailAvailable(Registration registration);

        /// <summary>
        /// Params `email` and `username` must not be null at once.
        /// Returns an Account corresponding to `email` or `username`.
        /// </summary>
        Task<Account> GetAccountByEmailOrUsername([MaybeNull] string email,[MaybeNull] string username = default, bool isConfirmed = true);

        /// <summary>
        /// Returns null if error, true if success, false if failed.
        /// </summary>
        Task<bool?> InsertToDatabase(Account account);
        
        /// <summary>
        /// Returns null if error, true if success, false if failed.
        /// </summary>
        Task<bool?> CreateRolesForAccountById(string accountId);
        
        /// <summary>
        /// Returns null if error, true if success, false if failed.
        /// </summary>
        Task<bool?> UpdateAccount(Account account);

        /// <summary>
        /// Returns null if error, true if success, false if failed.
        /// </summary>
        Task<bool?> UpdateStudent(Student student);
        
        /// <summary>
        /// Returns null if error, true if success, false if failed.
        /// </summary>
        Task<bool?> UpdateTeacher(Teacher teacher);
        
        Task<Student> GetStudentByAccountId(string accountId);
        
        Task<Teacher> GetTeacherByAccountId(string accountId);
        
        Task<Account> GetAccountById(string accountId, bool emailConfirmed = true);
        
        Task<bool?> IsStudentInfoAssociatedWithAccount(string studentId, string accountId);
        
        Task<bool?> IsTeacherInfoAssociatedWithAccount(string teacherId, string accountId);
    }
}