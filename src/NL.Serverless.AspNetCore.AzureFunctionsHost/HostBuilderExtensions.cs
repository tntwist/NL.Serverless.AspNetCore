using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace NL.Serverless.AspNetCore.AzureFunctionsHost
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureWebAppFunctionHost<TStartup>(this IHostBuilder builder) where TStartup : class
        {
            try 
            {
                var factory = new FunctionsWebApplicationFactory<TStartup>();
                var client = factory.CreateClient(new WebApplicationFactoryClientOptions
                {                    
                    AllowAutoRedirect = false
                });

                var requestHandler = new FunctionsRequestHandler(client);

                return builder.ConfigureServices(services => 
                {
                    services.AddSingleton<IFunctionsRequestHandler>(requestHandler);
                });
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);

                // log startup exceptions to application insights if instrumentation key is available.
                var appInsightsInstrumentationKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY", EnvironmentVariableTarget.Process);
                if (!string.IsNullOrWhiteSpace(appInsightsInstrumentationKey))
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    // TODO: Check if the telemetry client can be resolved somehow.
                    var telemetryClient = new TelemetryClient { InstrumentationKey = appInsightsInstrumentationKey };
#pragma warning restore CS0618 // Type or member is obsolete

                    telemetryClient.TrackException(e);
                }

                throw;
            }
        }
    }
}
