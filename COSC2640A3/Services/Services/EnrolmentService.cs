using COSC2640A3.DbContexts;
using COSC2640A3.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace COSC2640A3.Services.Services {

    public sealed class EnrolmentService : IEnrolmentService {
        
        private readonly ILogger<EnrolmentService> _logger;
        private readonly MainDbContext _dbContext;

        public EnrolmentService(
            ILogger<EnrolmentService> logger,
            MainDbContext dbContext
        ) {
            _logger = logger;
            _dbContext = dbContext;
        }
    }
}