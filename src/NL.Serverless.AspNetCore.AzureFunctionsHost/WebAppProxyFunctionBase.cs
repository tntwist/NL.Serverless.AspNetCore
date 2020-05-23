using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace NL.Serverless.AspNetCore.AzureFunctionsHost
{

    /// <summary>
    /// Abstract class for proxy functions to delegate incoming http request to a web app.
    /// </summary>
    public abstract class WebAppProxyFunctionBase
    {
        private readonly IFunctionsRequestHandler _requestHandler;

        public WebAppProxyFunctionBase(IFunctionsRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        protected virtual async Task<IActionResult> ProcessRequestAsync(
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("WebHostFunction is processing a request.");

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
