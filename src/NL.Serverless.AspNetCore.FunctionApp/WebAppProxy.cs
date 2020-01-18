using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NL.Serverless.AspNetCore.AzureFunctionsHost;
using System;
using System.Threading.Tasks;

namespace NL.Serverless.AspNetCore.FunctionApp
{
    /// <summary>
    /// Proxy function to delegate incoming http request to web app.
    /// </summary>
    public class WebAppProxy
    {
        private readonly IFunctionsRequestHandler _requestHandler;

        public WebAppProxy(IFunctionsRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        [FunctionName("Function1")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "patch", "delete",  Route = "{*any}")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("WebAppProxy processed a request.");

            try
            {
                await _requestHandler.HandleRequestAsync(req);
            }
            catch (Exception e) 
            {
                // logs details of exceptions occuring in the web app.
                log.LogError(e.ToString());
                throw e;
            }

            // return dummy result since request is handled by the web app.
            return new EmptyResult();
        }
    }
}
