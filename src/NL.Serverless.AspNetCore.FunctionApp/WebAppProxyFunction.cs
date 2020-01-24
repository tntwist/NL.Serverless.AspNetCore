using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NL.Serverless.AspNetCore.AzureFunctionsHost;
using System.Threading.Tasks;

namespace NL.Serverless.AspNetCore.FunctionApp
{
    /// <summary>
    /// Proxy function to delegate incoming http request to web app.
    /// </summary>
    public class WebAppProxyFunction : WebAppProxyFunctionBase
    {
        public WebAppProxyFunction(IFunctionsRequestHandler requestHandler) : base(requestHandler)
        {
        }

        [FunctionName("WebAppProxyFunction")]
        public async Task<IActionResult> Run(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "patch", "delete", "options", Route = "{*any}")] HttpRequest req,
           ILogger log)
        {
            return await ProcessRequestAsync(req, log);
        }
    }
}
