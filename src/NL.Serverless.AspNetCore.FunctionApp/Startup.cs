using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using NL.Serverless.AspNetCore.FunctionApp;
using System;
using System.Diagnostics;
using System.IO;

[assembly: FunctionsStartup(typeof(Startup))]
namespace NL.Serverless.AspNetCore.FunctionApp
{
    /// <summary>
    /// Startup class for the function app.
    /// </summary>
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // create config and hosting environment
            var configRoot = new ConfigurationBuilder()
               .SetBasePath(Environment.CurrentDirectory)
               .AddEnvironmentVariables()
               .Build();

            var config = configRoot.GetWebJobsRootConfiguration();

            var currentDirectory = Environment.CurrentDirectory;
            var webRootPath = Path.Combine(currentDirectory, "wwwroot");
            var hostingEnv = new HostingEnvironment()
            {
                ContentRootPath = currentDirectory,
                WebRootPath = webRootPath,
                ContentRootFileProvider = new PhysicalFileProvider(currentDirectory),
                WebRootFileProvider = new PhysicalFileProvider(webRootPath)
            };

            // configure services for web app.
            var webAppServices = new ServiceCollection();
            webAppServices.AddSingleton<DiagnosticSource>(new DiagnosticListener("Microsoft.AspNetCore"));
            webAppServices.AddSingleton<ObjectPoolProvider>(new DefaultObjectPoolProvider());
            webAppServices.AddSingleton<IApplicationLifetime, ApplicationLifetime>();
            webAppServices.AddSingleton<IHostingEnvironment>(hostingEnv);
            webAppServices.AddSingleton(config);

            // startup webapp
            var webAppStartUp = new WebApp.Startup(config);
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
    }
}
