using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using MyCompany.MyProject.Web.FunctionApp;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace MyCompany.MyProject.Web.FunctionApp
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
                // create config and hosting environment
                var configRoot = new ConfigurationBuilder()
                   .SetBasePath(Environment.CurrentDirectory)
                   .AddEnvironmentVariables()
                   .Build();

                // get content and webroot path for the function app.
                var assemblyPath = new DirectoryInfo(Path.GetDirectoryName(typeof(Startup).Assembly.Location));
                var contentRootPath = assemblyPath.Parent.FullName;
                var webRootPath = Path.Combine(contentRootPath, "wwwroot");

                var hostingEnv = new HostingEnvironment()
                {
                    ContentRootPath = contentRootPath,
                    WebRootPath = webRootPath,
                    ContentRootFileProvider = new PhysicalFileProvider(contentRootPath),
                    WebRootFileProvider = new PhysicalFileProvider(webRootPath)
                };

                // configure services for web app.
                var webAppServices = new ServiceCollection();
                webAppServices.AddSingleton<DiagnosticSource>(new DiagnosticListener("Microsoft.AspNetCore"));
                webAppServices.AddSingleton<ObjectPoolProvider>(new DefaultObjectPoolProvider());
                webAppServices.AddSingleton<IApplicationLifetime, ApplicationLifetime>();
                webAppServices.AddSingleton<IHostingEnvironment>(hostingEnv);

                // startup webapp
                var webAppStartUp = new Host.Startup.Startup(hostingEnv);
                var serviceProvider = webAppStartUp.ConfigureServices(webAppServices);

                //var serviceProvider = webAppServices.BuildServiceProvider();
                var appBuilder = new ApplicationBuilder(serviceProvider, new FeatureCollection());

                webAppStartUp.Configure(appBuilder);

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
            }
        }
    }
}
