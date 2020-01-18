using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.ObjectPool;
using Your.New.ProjectName.FunctionApp;
using System;
using System.Diagnostics;
using System.IO;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Your.New.ProjectName.FunctionApp
{
    /// <summary>
    /// Startup class for the function app.
    /// </summary>
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            try
            {
                // get content and webroot path for the function app.
                var assemblyPath = new DirectoryInfo(Path.GetDirectoryName(typeof(Startup).Assembly.Location));
                var contentRootPath = assemblyPath.Parent.FullName;
                var webRootPath = Path.Combine(contentRootPath, "wwwroot");

                var functionHostingEnv = builder.Services.BuildServiceProvider().GetService<IHostEnvironment>();

                var hostingEnv = new FunctionAppHostingEnvironment()
                {
                    ContentRootPath = contentRootPath,
                    WebRootPath = webRootPath,
                    ContentRootFileProvider = new PhysicalFileProvider(contentRootPath),
                    WebRootFileProvider = new PhysicalFileProvider(webRootPath),
                    ApplicationName = typeof(WebApp.Startup).Assembly.FullName,
                    EnvironmentName = functionHostingEnv.EnvironmentName
                };

                // create config and hosting environment
                var config = new ConfigurationBuilder()
                   .SetBasePath(contentRootPath)
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .AddJsonFile($"appsettings.{hostingEnv.EnvironmentName}.json", optional: true, reloadOnChange: true)
                   .AddEnvironmentVariables();

                if (hostingEnv.IsDevelopment())
                {
                    config.AddUserSecrets(typeof(WebApp.Startup).Assembly, optional: true);
                };

                var configRoot = config.Build();

                // configure services for web app.
                var webAppServices = new ServiceCollection();
                var diagnosticSource = new DiagnosticListener(hostingEnv.ApplicationName);
                webAppServices.AddSingleton<DiagnosticSource>(diagnosticSource);
                webAppServices.AddSingleton(diagnosticSource);
                webAppServices.AddSingleton<ObjectPoolProvider>(new DefaultObjectPoolProvider());
                webAppServices.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>();
                webAppServices.AddSingleton<IWebHostEnvironment>(hostingEnv);
                webAppServices.AddSingleton<IConfiguration>(configRoot);

                // startup webapp
                var webAppStartUp = new WebApp.Startup(configRoot);
                webAppStartUp.ConfigureServices(webAppServices);

                var serviceProvider = webAppServices.BuildServiceProvider();

                var appBuilder = new ApplicationBuilder(serviceProvider, new FeatureCollection());

                webAppStartUp.Configure(appBuilder, hostingEnv);

                // create request delegate from the configured app builder
                var requestDelegate = appBuilder.Build();

                builder.Services.AddSingleton(requestDelegate);

                // create instance of WebApp ServiceProvider for resolving the service provider for the web app over DI.
                var webAppServiceProvider = new WebAppServiceProvider
                {
                    ServiceProvider = serviceProvider
                };

                builder.Services.AddSingleton(webAppServiceProvider);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                // log exception to app insights if instrumentation key is available.
                var appInsightsInstrumentationKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY", EnvironmentVariableTarget.Process);
                if (!string.IsNullOrWhiteSpace(appInsightsInstrumentationKey))
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    var telemetryClient = new TelemetryClient { InstrumentationKey = appInsightsInstrumentationKey };
#pragma warning restore CS0618 // Type or member is obsolete

                    telemetryClient.TrackException(e);
                }

                throw e;
            }
        }
    }
}
