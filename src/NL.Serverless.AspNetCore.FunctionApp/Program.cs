using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NL.Serverless.AspNetCore.AzureFunctionsHost;
using Microsoft.AspNetCore.Hosting;

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
                .ConfigureFunctionsWorkerDefaults()
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
