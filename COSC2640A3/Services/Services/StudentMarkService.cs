using COSC2640A3.DbContexts;
using COSC2640A3.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace COSC2640A3.Services.Services {

    public sealed class StudentMarkService : IStudentMarkService {
        
        private readonly ILogger<StudentMarkService> _logger;
        private readonly MainDbContext _dbContext;

        public StudentMarkService(
            ILogger<StudentMarkService> logger,
            MainDbContext dbContext
        ) {
            _logger = logger;
            _dbContext = dbContext;
        }
    }
}