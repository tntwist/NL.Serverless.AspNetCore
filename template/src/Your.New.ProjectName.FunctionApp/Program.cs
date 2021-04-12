using Microsoft.Extensions.Hosting;
using NL.Serverless.AspNetCore.AzureFunctionsHost;
using System.Threading.Tasks;

namespace Your.New.ProjectName.FunctionApp
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
                .ConfigureWebAppFunctionHost<WebApp.Startup>()
                .Build();

            await host.RunAsync();
        }
    }
}
