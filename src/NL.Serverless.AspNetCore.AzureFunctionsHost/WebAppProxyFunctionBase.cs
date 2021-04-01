using Microsoft.Azure.Functions.Worker.Http;
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

        protected virtual async Task<HttpResponseData> ProcessRequestAsync(
            HttpRequestData req,
            ILogger log)
        {
            log.LogInformation("WebAppProxyFunction is processing a request.");

            try
            {
                return await _requestHandler.HandleRequestAsync(req);
            }
            catch (Exception e)
            {
                // logs details of exceptions occuring in the web app.
                log.LogError(e.ToString());
                throw;
            }
        }
    }
}
