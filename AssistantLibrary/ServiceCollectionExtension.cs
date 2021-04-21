using AssistantLibrary.Interfaces;
using AssistantLibrary.Models;
using AssistantLibrary.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AssistantLibrary {

    public static class ServiceCollectionExtension {

        public static void RegisterAssistantLibrary(this IServiceCollection services, IConfiguration configuration) {

            services.Configure<AssistantOptions>(configuration.GetSection(nameof(AssistantLibrary)));

            services.AddScoped<IGoogleService, GoogleService>();
        }
    }
}