using COSC2640A3.DbContexts;
using COSC2640A3.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace COSC2640A3.Services.Services {

    public sealed class ClassroomService : IClassroomService {
        
        private readonly ILogger<ClassroomService> _logger;
        private readonly MainDbContext _dbContext;

        public ClassroomService(
            ILogger<ClassroomService> logger,
            MainDbContext dbContext
        ) {
            _logger = logger;
            _dbContext = dbContext;
        }
    }
}