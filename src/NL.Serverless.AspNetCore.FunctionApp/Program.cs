using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Azure.Functions.Worker.Pipeline;
using NL.Serverless.AspNetCore.AzureFunctionsHost;

namespace NL.Serverless.AspNetCore.FunctionApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //#if DEBUG
            //            Debugger.Launch();
            //#endif
            var host = new HostBuilder()
                .ConfigureAppConfiguration(c =>
                {
                    c.AddCommandLine(args);
                })
                .ConfigureFunctionsWorker((c, b) =>
                {
                    b.UseFunctionExecutionMiddleware();
                })
                .ConfigureServices(services => 
                {
                    var webAppStartup = new FunctionsHostStartup<WebApp.Startup>();
                    webAppStartup.Configure(services);
                })
                .Build();

            await host.RunAsync();
        }
    }
}
