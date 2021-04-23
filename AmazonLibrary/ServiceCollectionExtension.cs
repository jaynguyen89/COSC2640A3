using AmazonLibrary.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AmazonLibrary {

    public static class ServiceCollectionExtension {

        public static void RegisterAmazonLibrary(this IServiceCollection services, IConfiguration configuration) {

            services.Configure<AmazonOptions>(configuration.GetSection(nameof(AmazonOptions)));
            services.AddSingleton<DynamoDbContext>();
            services.AddSingleton<AmazonS3Context>();
        }
    }
}
