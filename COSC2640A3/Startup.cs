using System;
using AmazonLibrary;
using AssistantLibrary;
using COSC2640A3.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace COSC2640A3 {

    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            services.AddCors();
            services.AddControllers();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options => {
                        options.Audience = Configuration.GetValue<string>($"{ nameof(MainOptions) }:{ nameof(MainOptions.UserPoolAppClientId)}");
                        options.Authority = Configuration.GetValue<string>($"{ nameof(MainOptions) }:{ nameof(MainOptions.PoolIdentityProviderUrl)}");
                    });

            services.AddAuthorization(options => {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser().Build();
            });
            
            services.AddMvc(options => options.EnableEndpointRouting = false)
                    .AddSessionStateTempDataProvider()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            
            services.AddSession(options => {
                options.Cookie = new CookieBuilder {
                    Domain = nameof(COSC2640A3),
                    Expiration = TimeSpan.FromDays(int.Parse(Configuration.GetValue<string>($"{ nameof(MainOptions) }:{ nameof(MainOptions.CookieAgeTimespan)}"))),
                    HttpOnly = bool.Parse(Configuration.GetValue<string>($"{ nameof(MainOptions) }:{ nameof(MainOptions.CookieHttpOnly)}")),
                    IsEssential = bool.Parse(Configuration.GetValue<string>($"{ nameof(MainOptions) }:{ nameof(MainOptions.CookieEssential)}")),
                    Name = nameof(COSC2640A3),
                    SameSite = SameSiteMode.None,
                    Path = Configuration.GetValue<string>($"{ nameof(MainOptions) }:{ nameof(MainOptions.CookiePath)}"),
                    MaxAge = TimeSpan.FromDays(int.Parse(Configuration.GetValue<string>($"{ nameof(MainOptions) }:{ nameof(MainOptions.CookieAgeTimespan)}")))
                };
            });
            services.AddHttpContextAccessor();
            
            services.Configure<MainOptions>(Configuration.GetSection(nameof(MainOptions)));

            services.RegisterAssistantLibrary(Configuration);
            services.RegisterAmazonLibrary(Configuration);
            services.RegisterCoreServices(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory) {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            var loggerConfig = Configuration.GetAWSLoggingConfigSection();
            _ = loggerFactory.AddAWSProvider(loggerConfig);
            
            app.UseHttpsRedirection();
            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}"
                );
            });
        }
    }
}
