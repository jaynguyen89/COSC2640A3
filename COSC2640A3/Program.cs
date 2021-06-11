using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Helper.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using Amazon.Runtime;

namespace COSC2640A3 {

    public class Program {

        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(webBuilder => {
                    webBuilder.AddJsonFile("appsettings.json", true, true);
                    webBuilder.AddEnvironmentVariables();
                    _ = webBuilder.AddSystemsManager(
                        $"{ SharedConstants.FSlash }{ nameof(COSC2640A3) }",
                        new AWSOptions {
                            Region = RegionEndpoint.APSoutheast2,
                            Credentials = new BasicAWSCredentials("AKIAJSENDXCAPZWGB6HQ", "HeGULGolRgnxwKIIm4K2d8E+sAoHVBukvR+5umU3")
                        },
                        false,
                        TimeSpan.FromMinutes(10)
                    );
                    
                    _ = webBuilder.AddSystemsManager(
                        $"{ SharedConstants.FSlash }{ nameof(AmazonLibrary) }",
                        new AWSOptions {
                            Region = RegionEndpoint.APSoutheast2,
                            Credentials = new BasicAWSCredentials("AKIAJSENDXCAPZWGB6HQ", "HeGULGolRgnxwKIIm4K2d8E+sAoHVBukvR+5umU3")
                        },
                        false,
                        TimeSpan.FromMinutes(10)
                    );
                    
                    _ = webBuilder.AddSystemsManager(
                        $"{ SharedConstants.FSlash }{ nameof(AssistantLibrary) }",
                        new AWSOptions {
                            Region = RegionEndpoint.APSoutheast2,
                            Credentials = new BasicAWSCredentials("AKIAJSENDXCAPZWGB6HQ", "HeGULGolRgnxwKIIm4K2d8E+sAoHVBukvR+5umU3")
                        },
                        false,
                        TimeSpan.FromMinutes(10)
                    );
                })
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
