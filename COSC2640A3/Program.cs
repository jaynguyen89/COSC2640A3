using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Helper.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace COSC2640A3 {

    public class Program {

        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(webBuilder => {
                    webBuilder.AddEnvironmentVariables();
                    _ = webBuilder.AddSystemsManager(
                        $"{ SharedConstants.FSlash }{ nameof(COSC2640A3) }",
                        new AWSOptions { Region = RegionEndpoint.APSoutheast2 },
                        false,
                        TimeSpan.FromMinutes(10)
                    );
                    
                    _ = webBuilder.AddSystemsManager(
                        $"{ SharedConstants.FSlash }{ nameof(AmazonLibrary) }",
                        new AWSOptions { Region = RegionEndpoint.APSoutheast2 },
                        false,
                        TimeSpan.FromMinutes(10)
                    );
                    
                    _ = webBuilder.AddSystemsManager(
                        $"{ SharedConstants.FSlash }{ nameof(AssistantLibrary) }",
                        new AWSOptions { Region = RegionEndpoint.APSoutheast2 },
                        false,
                        TimeSpan.FromMinutes(10)
                    );
                })
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
