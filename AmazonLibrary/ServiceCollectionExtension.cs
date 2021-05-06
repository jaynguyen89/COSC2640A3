using AmazonLibrary.Contexts;
using AmazonLibrary.Interfaces;
using AmazonLibrary.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AmazonLibrary {

    public static class ServiceCollectionExtension {

        public static void RegisterAmazonLibrary(this IServiceCollection services, IConfiguration configuration) {

            services.Configure<AmazonOptions>(configuration.GetSection(nameof(AmazonOptions)));
            services.AddSingleton<DynamoDbContext>();
            services.AddSingleton<AmazonS3Context>();
            services.AddSingleton<AmazonTextractContext>();

            services.AddScoped<IS3Service, S3Service>();
            services.AddScoped<IDynamoService, DynamoService>();
            services.AddScoped<IAmazonMailService, AmazonMailService>();
            services.AddScoped<ITextractService, TextractService>();
        }
    }
}
