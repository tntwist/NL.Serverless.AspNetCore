using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using NL.Serverless.AspNetCore.AzureFunctionsHost;
using Your.New.ProjectName.FunctionApp;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Your.New.ProjectName.FunctionApp
{
    /// <summary>
    /// Startup class for the function app.
    /// </summary>
    internal class Startup : FunctionsHostStartup<WebApp.Startup>
    {
    }
}
