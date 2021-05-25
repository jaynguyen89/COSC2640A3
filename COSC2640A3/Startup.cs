using System;
using System.IO;
using System.Reflection;
using AmazonLibrary;
using AssistantLibrary;
using COSC2640A3.Attributes;
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
using Microsoft.OpenApi.Models;

namespace COSC2640A3 {

    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            services.AddCors();
            services.AddControllers();
            
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "COSC2640 A3 API",
                    Description = "The documentation and testing tool for COSC2640 A3 Microservices API.",
                    Version = "v1.0"
                });
                
                var xmlFile = $"{ Assembly.GetExecutingAssembly().GetName().Name }.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
                
                options.OperationFilter<SwaggerXmlFormatter>();
            });

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
            
            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication1 v1"));

            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

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
