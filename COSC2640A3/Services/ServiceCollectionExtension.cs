using COSC2640A3.DbContexts;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace COSC2640A3.Services {

    public static class ServiceCollectionExtension {

        public static void RegisterCoreServices(this IServiceCollection services, IConfiguration configuration) {

            services.AddScoped<MainDbContext>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var isDevelopment = configuration.GetSection($"{nameof(COSC2640A3)}Environment").Value.Equals("Development");
            
            services.AddStackExchangeRedisCache(options => {
                options.Configuration = configuration.GetValue<string>(
                    isDevelopment ? $"{ nameof(MainOptions) }:{ nameof(MainOptions.DevelopmentCacheEndpoint) }"
                                  : $"{ nameof(MainOptions) }:{ nameof(MainOptions.ProductionCacheEndpoint) }"
                );
                
                options.InstanceName = configuration.GetValue<string>(nameof(MainOptions.CacheStoreName));
            });

            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddScoped<IContextService, ContextService>();
            
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IRoleService, RoleService>();
            
            services.AddScoped<IClassroomService, ClassroomService>();
            services.AddScoped<IEnrolmentService, EnrolmentService>();
            services.AddScoped<IStudentMarkService, StudentMarkService>();
        }
    }
}