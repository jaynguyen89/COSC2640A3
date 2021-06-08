using System.Linq;
using System.Threading.Tasks;
using COSC2640A3.DbContexts;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace COSC2640A3.Services.Services {

    public sealed class GeneratorService : IGeneratorService {
        
        private readonly ILogger<GeneratorService> _logger;
        private readonly MainDbContext _dbContext;

        public GeneratorService(
            ILogger<GeneratorService> logger,
            MainDbContext dbContext
        ) {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<string[]> InsertMultipleAccounts(Account[] accounts) {
            _logger.LogInformation($"{ nameof(GeneratorService) }.{ nameof(InsertMultipleAccounts) }: Inserting accounts.");
            await _dbContext.Accounts.AddRangeAsync(accounts);

            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0 ? accounts.Select(account => account.Id).ToArray() : null;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(GeneratorService) }.{ nameof(InsertMultipleAccounts) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return null;
            }
        }

        public async Task<bool?> InsertMultipleRoles(AccountRole[] accountRoles) {
            _logger.LogInformation($"{ nameof(GeneratorService) }.{ nameof(InsertMultipleRoles) }: Inserting roles.");
            await _dbContext.AccountRoles.AddRangeAsync(accountRoles);

            try {
                var result = await _dbContext.SaveChangesAsync();
                return result == accountRoles.Length;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(GeneratorService) }.{ nameof(InsertMultipleRoles) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<string[]> InsertMultipleStudents(Student[] students) {
            _logger.LogInformation($"{ nameof(GeneratorService) }.{ nameof(InsertMultipleStudents) }: Inserting students.");
            await _dbContext.Students.AddRangeAsync(students);

            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0 ? students.Select(student => student.Id).ToArray() : null;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(GeneratorService) }.{ nameof(InsertMultipleStudents) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<string[]> InsertMultipleTeachers(Teacher[] teachers) {
            _logger.LogInformation($"{ nameof(GeneratorService) }.{ nameof(InsertMultipleTeachers) }: Inserting teachers.");
            await _dbContext.Teachers.AddRangeAsync(teachers);

            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0 ? teachers.Select(teacher => teacher.Id).ToArray() : null;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(GeneratorService) }.{ nameof(InsertMultipleTeachers) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<bool?> InsertMultipleEnrolments(Enrolment[] enrolments) {
            _logger.LogInformation($"{ nameof(GeneratorService) }.{ nameof(InsertMultipleEnrolments) }: Inserting enrolments.");
            await _dbContext.Enrolments.AddRangeAsync(enrolments);

            try {
                var result = await _dbContext.SaveChangesAsync();
                return result == enrolments.Length;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(GeneratorService) }.{ nameof(InsertMultipleEnrolments) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public async Task<string[]> InsertMultipleClassrooms(Classroom[] classrooms) {
            _logger.LogInformation($"{ nameof(GeneratorService) }.{ nameof(InsertMultipleClassrooms) }: Inserting classrooms.");
            await _dbContext.Classrooms.AddRangeAsync(classrooms);

            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0 ? classrooms.Select(classroom => classroom.Id).ToArray() : null;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(GeneratorService) }.{ nameof(InsertMultipleClassrooms) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }

        public bool? InsertMultipleInvoices(Invoice[] invoices) {
            _logger.LogInformation($"{ nameof(GeneratorService) }.{ nameof(InsertMultipleInvoices) }: Inserting classrooms.");
            _dbContext.Invoices.AddRange(invoices);

            try {
                var result = _dbContext.SaveChanges();
                return result == invoices.Length;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(GeneratorService) }.{ nameof(InsertMultipleInvoices) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }
    }
}