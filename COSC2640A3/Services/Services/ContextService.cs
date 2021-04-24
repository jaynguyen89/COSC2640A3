using System.Threading.Tasks;
using COSC2640A3.DbContexts;
using COSC2640A3.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace COSC2640A3.Services.Services {

    public sealed class ContextService : IContextService {
        
        private readonly ILogger<ContextService> _logger;
        private readonly MainDbContext _dbContext;

        public ContextService(
            ILogger<ContextService> logger,
            MainDbContext dbContext
        ) {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task StartTransaction() {
            _logger.LogInformation("DB Transaction begins.");
            await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task ConfirmTransaction() {
            _logger.LogInformation("DB Transaction commits.");
            await _dbContext.Database.CommitTransactionAsync();
        }

        public async Task RevertTransaction() {
            _logger.LogInformation("DB Transaction reverts.");
            await _dbContext.Database.RollbackTransactionAsync();
        }
    }
}