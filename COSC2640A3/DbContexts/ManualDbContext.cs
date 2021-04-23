using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace COSC2640A3.DbContexts {

    public partial class MainDbContext {

        private readonly IOptions<MainOptions> _options;
        private readonly IConfiguration _configuration;

        public MainDbContext(
            IOptions<MainOptions> options,
            IConfiguration configuration
        ) {
            _options = options;
            _configuration = configuration;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);

            var connectionString =
                _configuration.GetSection($"{ nameof(COSC2640A3) }Environment").Value.Equals("Development")
                    ? _options.Value.DevelopmentConnectionString
                    : _options.Value.ProductionConnectionString;

            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(connectionString);
        }
    }
}