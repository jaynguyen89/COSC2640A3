using COSC2640A3.DbContexts;
using COSC2640A3.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace COSC2640A3.Services.Services {

    public sealed class AssessmentService : IAssessmentService {
        
        private readonly ILogger<AssessmentService> _logger;
        private readonly MainDbContext _dbContext;

        public AssessmentService(
            ILogger<AssessmentService> logger,
            MainDbContext dbContext
        ) {
            _logger = logger;
            _dbContext = dbContext;
        }
    }
}