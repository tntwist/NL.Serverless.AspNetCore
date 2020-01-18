using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using NL.Serverless.AspNetCore.AzureFunctionsHost;
using NL.Serverless.AspNetCore.FunctionApp;

[assembly: FunctionsStartup(typeof(Startup))]
namespace NL.Serverless.AspNetCore.FunctionApp
{
    /// <summary>
    /// Startup class for the function app.
    /// </summary>
    internal class Startup : FunctionsHostStartup<WebApp.Startup>
    {
    }
}
