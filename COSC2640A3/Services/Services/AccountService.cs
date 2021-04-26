using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using COSC2640A3.Bindings;
using COSC2640A3.DbContexts;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace COSC2640A3.Services.Services {

    public sealed class AccountService : IAccountService {
        
        private readonly ILogger<AccountService> _logger;
        private readonly MainDbContext _dbContext;

        public AccountService(
            ILogger<AccountService> logger,
            MainDbContext dbContext
        ) {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<KeyValuePair<bool?, string>> IsUsernameAndEmailAvailable(Registration registration) {
            try {
                _logger.LogInformation($"{ nameof(AuthenticationService) }.{ nameof(IsUsernameAndEmailAvailable) }: Check registration data against { nameof(MainDbContext) }.");
                
                var isEmailTaken = await _dbContext.Accounts.AnyAsync(account => account.EmailAddress.Equals(registration.Email));
                if (isEmailTaken) return new KeyValuePair<bool?, string>(false, nameof(Registration.Email));

                var isUsernameTaken = await _dbContext.Accounts.AnyAsync(account => account.NormalizedUsername.Equals(registration.Username.ToUpper()));
                return isUsernameTaken
                    ? new KeyValuePair<bool?, string>(false, nameof(Registration.Username))
                    : new KeyValuePair<bool?, string>(true, default);
            }
            catch (ArgumentNullException e) {
                _logger.LogError($"{ nameof(AccountService) }.{ nameof(IsUsernameAndEmailAvailable) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return new KeyValuePair<bool?, string>(default, default);
            }
        }

        public async Task<Account> GetAccountByEmailOrUsername(string email, string username, bool isConfirmed = true) {
            try {
                return !Helpers.IsProperString(username)
                    ? await _dbContext.Accounts.SingleOrDefaultAsync(account => account.EmailAddress.Equals(email) && account.EmailConfirmed == isConfirmed)
                    : await _dbContext.Accounts.SingleOrDefaultAsync(account => account.NormalizedUsername.Equals(username.ToUpper()) && account.EmailConfirmed == isConfirmed);
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(AccountService) }.{ nameof(GetAccountByEmailOrUsername) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (InvalidOperationException e) {
                _logger.LogError($"{ nameof(AccountService) }.{ nameof(GetAccountByEmailOrUsername) } - { nameof(InvalidOperationException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<bool?> InsertToDatabase(Account account) {
            await _dbContext.Accounts.AddAsync(account);

            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(AccountService) }.{ nameof(InsertToDatabase) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<bool?> UpdateAccount(Account account) {
            _dbContext.Accounts.Update(account);

            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(AccountService) }.{ nameof(UpdateAccount) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<bool?> UpdateStudent(Student student) {
            _dbContext.Students.Update(student);

            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(AccountService) }.{ nameof(UpdateStudent) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<bool?> UpdateTeacher(Teacher teacher) {
            _dbContext.Teachers.Update(teacher);

            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(AccountService) }.{ nameof(UpdateTeacher) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<Student> GetStudentByAccountId(string accountId) {
            try {
                return await _dbContext.Students.SingleOrDefaultAsync(student => student.AccountId.Equals(accountId) && student.Account.EmailConfirmed);
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(AccountService) }.{ nameof(GetStudentByAccountId) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (InvalidOperationException e) {
                _logger.LogError($"{ nameof(AccountService) }.{ nameof(GetStudentByAccountId) } - { nameof(InvalidOperationException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<Teacher> GetTeacherByAccountId(string accountId) {
            try {
                return await _dbContext.Teachers.SingleOrDefaultAsync(teacher => teacher.AccountId.Equals(accountId) && teacher.Account.EmailConfirmed);
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(AccountService) }.{ nameof(GetTeacherByAccountId) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (InvalidOperationException e) {
                _logger.LogError($"{ nameof(AccountService) }.{ nameof(GetTeacherByAccountId) } - { nameof(InvalidOperationException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<Account> GetAccountById(string accountId, bool emailConfirmed = true) {
            try {
                return await _dbContext.Accounts.SingleOrDefaultAsync(account => account.Id.Equals(accountId) && account.EmailConfirmed == emailConfirmed);
            }
            catch (ArgumentNullException e) {
                _logger.LogWarning($"{ nameof(AccountService) }.{ nameof(GetAccountById) } - { nameof(ArgumentNullException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
            catch (InvalidOperationException e) {
                _logger.LogError($"{ nameof(AccountService) }.{ nameof(GetAccountById) } - { nameof(InvalidOperationException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }
    }
}