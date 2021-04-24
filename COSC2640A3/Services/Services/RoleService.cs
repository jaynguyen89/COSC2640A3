using System.Threading.Tasks;
using COSC2640A3.DbContexts;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using Helper.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace COSC2640A3.Services.Services {

    public sealed class RoleService : IRoleService {
        
        private readonly ILogger<RoleService> _logger;
        private readonly MainDbContext _dbContext;

        public RoleService(
            ILogger<RoleService> logger,
            MainDbContext dbContext
        ) {
            _logger = logger;
            _dbContext = dbContext;
        }


        public async Task<bool?> CreateRolesForAccountById(string accountId) {
            var studentRole = new AccountRole {
                AccountId = accountId,
                Role = (byte) SharedEnums.Role.Student
            };

            var teacherRole = new AccountRole {
                AccountId = accountId,
                Role = (byte) SharedEnums.Role.Teacher
            };

            await _dbContext.AccountRoles.AddRangeAsync(new[] { studentRole, teacherRole });
            
            var student = new Student { AccountId = accountId };
            var teacher = new Teacher { AccountId = accountId };

            await _dbContext.Students.AddAsync(student);
            await _dbContext.Teachers.AddAsync(teacher);

            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0;
            }
            catch (DbUpdateException e) {
                _logger.LogError($"{ nameof(RoleService) }.{ nameof(CreateRolesForAccountById) } - { nameof(DbUpdateException) }: { e.Message }\n\n{ e.StackTrace }");
                return default;
            }
        }
    }
}