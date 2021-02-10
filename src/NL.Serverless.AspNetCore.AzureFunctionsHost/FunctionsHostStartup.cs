using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NL.Serverless.AspNetCore.AzureFunctionsHost
{
    /// <summary>
    /// Configures and builds the Asp.Net Core application behind TStartup.
    /// </summary>
    /// <typeparam name="TStartup">Startup class of the ASP.Net Core application.</typeparam>
    public class FunctionsHostStartup<TStartup> where TStartup : class
    {
        /// <summary>
        /// Path to the content root of the Asp.Net Core Application.
        /// </summary>
        public string ApplicationContentRootPath { get; set; }

        /// <summary>
        /// Path to the web root of the Asp.Net Core Application.
        /// </summary>
        public string ApplicationWebRootPath { get; set; }

        /// <summary>
        /// Empty Constructor.
        /// </summary>
        public FunctionsHostStartup()
        {
            // get content and web root paths for ASP.Net Core Application.
            var startupAssemblyPath = new DirectoryInfo(Path.GetDirectoryName(typeof(TStartup).Assembly.Location));
            ApplicationContentRootPath = startupAssemblyPath.Parent.FullName;
            ApplicationWebRootPath = Path.Combine(ApplicationContentRootPath, "wwwroot");
        }

        /// <summary>
        /// Configures the ASP.Net Core application behind TStartup and registers an <see cref="IFunctionsRequestHandler" /> to the FunctionsHostBuilder.
        /// </summary>
        /// <param name="services"></param>
        public void Configure(IServiceCollection services)
        {
            try
            {
                var functionsRequestHandler = BuildFunctionsRequestHandler(services);
                services.AddSingleton(functionsRequestHandler);
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

                throw e;
            }
        }

        private IFunctionsRequestHandler BuildFunctionsRequestHandler(IServiceCollection services)
        {
            FunctionsWebHostEnvironment webHostEnv;
            using (var functionsServiceProvider = services.BuildServiceProvider())
            {
                var functionsHostingEnv = functionsServiceProvider.GetRequiredService<IHostEnvironment>();

                webHostEnv = CreateWebHostEnvironment(functionsHostingEnv);
            }

            var config = CreateConfiguration(webHostEnv);

            ServiceCollection applicationServices = CreateBasicApplicationServices(webHostEnv, config);
            // Some libraries (like Microsoft.ApplicationInsights.AspNetCore) still need the deprecated hosting environment interface.
            applicationServices.AddSingleton<Microsoft.AspNetCore.Hosting.IHostingEnvironment>(webHostEnv);

            // build service collection used for creating an instance of TStartup.
            var startupServices = new ServiceCollection();
            startupServices.AddSingleton(config);
            startupServices.AddSingleton(webHostEnv);
            startupServices.AddSingleton<IHostEnvironment>(webHostEnv);

            TStartup startupInstance;
            using (var startupServiceProvider = startupServices.BuildServiceProvider())
            {
                startupInstance = ActivatorUtilities.CreateInstance<TStartup>(startupServiceProvider);
            }

            var startupMethods = typeof(TStartup).GetMethods().ToList();

            // get ConfigureServices method of TStartup
            var configureServicesMethod = startupMethods
                .SingleOrDefault(mi => string.Equals(mi.Name, "ConfigureServices", StringComparison.InvariantCulture));

            // invoke ConfigureService method from instance of TStartUp if exists.
            var configureServicesResult = configureServicesMethod?.Invoke(startupInstance, new object[] { applicationServices });

            // if the ConfigureService method returns a ServiceProvider we use this provider for the application. If not, we build it :)
            var applicationServiceProvider = configureServicesResult as IServiceProvider ?? applicationServices.BuildServiceProvider();

            var applicationBuilder = new ApplicationBuilder(applicationServiceProvider, new FeatureCollection());

            var configureMethod = startupMethods
                .SingleOrDefault(mi => string.Equals(mi.Name, "Configure", StringComparison.InvariantCulture));

            if (configureMethod != null)
            {
                InvokeConfigure(configureMethod, startupInstance, applicationBuilder);
            }

            var requestDelegate = applicationBuilder.Build();
            var functionRequestHandler = new FunctionsRequestHandler(applicationBuilder.ApplicationServices, requestDelegate);

            return functionRequestHandler;
        }

        /// <summary>
        /// Creates the web host environment for the ASP.Net Core application.
        /// </summary>
        /// <param name="functionsHostEnvironment">The host environment of the functions app.</param>
        /// <returns>IWebHostingEnvironment for the ASP.Net Core application.</returns>
        protected virtual FunctionsWebHostEnvironment CreateWebHostEnvironment(IHostEnvironment functionsHostEnvironment)
        {
            var hostingEnv = new FunctionsWebHostEnvironment()
            {
                ContentRootPath = ApplicationContentRootPath,
                ContentRootFileProvider = new PhysicalFileProvider(ApplicationContentRootPath),
                ApplicationName = typeof(TStartup).Assembly.FullName,
                EnvironmentName = functionsHostEnvironment.EnvironmentName
            };

            if (Directory.Exists(ApplicationWebRootPath))
            {
                hostingEnv.WebRootPath = ApplicationWebRootPath;
                hostingEnv.WebRootFileProvider = new PhysicalFileProvider(ApplicationWebRootPath);
            }
            else
            {
                // see: https://github.com/dotnet/aspnetcore/blob/f17fcfd3c80464d36cf632274ad02f04405efeba/src/Hosting/Hosting/src/Internal/HostingEnvironmentExtensions.cs#L59
                hostingEnv.WebRootFileProvider = new NullFileProvider();
            }

            return hostingEnv;
        }

        /// <summary>
        /// Creates the configuration for the ASP.Net Core application.
        /// Override this if you want to add other configuration sources.
        /// </summary>
        /// <param name="hostEnvironment">HostingEnvironment of the ASP.Net Core Applicationen</param>
        /// <returns>IConfiguration for the ASP.Net Core application</returns>
        protected virtual IConfiguration CreateConfiguration(IWebHostEnvironment hostEnvironment)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(hostEnvironment.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables();

            if (hostEnvironment.IsDevelopment())
            {
                config.AddUserSecrets(typeof(TStartup).Assembly, optional: true);
            };

            return config.Build();
        }

        /// <summary>
        /// Creates an service collection with basic services needed for the ASP.Net Core Application.
        /// Adds <see cref="DiagnosticListener"/>, <see cref="DiagnosticSource"/>, 
        /// <see cref="DefaultObjectPoolProvider"/>, <see cref="ApplicationLifetime"/>,
        /// <see cref="IWebHostEnvironment"/> and <see cref="IConfiguration"/>.
        /// </summary>
        /// <param name="webHostEnv">Hosting environment for the ASP.Net Core Application</param>
        /// <param name="config">Configuration for the ASP.Net Core Application</param>
        /// <returns>ServiceCollection for the ASP.Net Core Application.</returns>
        protected virtual ServiceCollection CreateBasicApplicationServices(IWebHostEnvironment hostEnvironment, IConfiguration config)
        {
            var applicationServices = new ServiceCollection();

            // add diagnostic listener.
            var diagnosticSource = new DiagnosticListener(hostEnvironment.ApplicationName);
            applicationServices.AddSingleton<DiagnosticSource>(diagnosticSource);
            applicationServices.AddSingleton(diagnosticSource);

            // add default object pool provider and application lifetime.
            applicationServices.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            applicationServices.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>();

            // add web host environment.
            applicationServices.AddSingleton(hostEnvironment);
            applicationServices.AddSingleton<IHostEnvironment>(hostEnvironment);

            //add configuration.
            applicationServices.AddSingleton(config);

            //Logging and options.
            applicationServices.AddLogging(loggingBuilder =>
            {
                ConfigureLogging(loggingBuilder, config);
            });
            applicationServices.AddOptions();

            return applicationServices;
        }

        /// <summary>
        /// Adds default logging for the ASP.Net Core Application.
        /// Override to configure logging for your usecase.
        /// See: https://github.com/dotnet/aspnetcore/blob/f17fcfd3c80464d36cf632274ad02f04405efeba/src/DefaultBuilder/src/WebHost.cs#L194
        /// </summary>
        /// <param name="loggingBuilder">The logging builder</param>
        /// <param name="config">The configuration of the ASP.Net Core Application</param>
        protected virtual void ConfigureLogging(ILoggingBuilder loggingBuilder, IConfiguration config)
        {
            loggingBuilder.AddConfiguration(config.GetSection("Logging"));
            loggingBuilder.AddConsole();
            loggingBuilder.AddDebug();
            loggingBuilder.AddEventSourceLogger();
        }

        /// <summary>
        /// Invokes the method on the given instance resolving the parameters from given service provider. 
        /// </summary>
        /// <param name="method">The method to invoke</param>
        /// <param name="instance">The instance of the object</param>
        /// <param name="applicationBuilder">The application builder to configure</param>
        private void InvokeConfigure(MethodInfo method, object instance, IApplicationBuilder applicationBuilder)
        {
            var parameterInfos = method.GetParameters();

            var parameters = new List<object>();
            foreach (var parameterInfo in parameterInfos)
            {
                // if type of parameter is assignable from IApplication builder, add application builder to parameters
                // else resolve the parameter form the services.
                if (parameterInfo.ParameterType.IsAssignableFrom(typeof(IApplicationBuilder)))
                    parameters.Add(applicationBuilder);
                else
                    parameters.Add(applicationBuilder.ApplicationServices.GetService(parameterInfo.ParameterType));

            }

            method.Invoke(instance, parameters.ToArray());
        }
    }
}
