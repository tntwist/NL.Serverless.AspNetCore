using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using NL.Serverless.AspNetCore.AzureFunctionsHost;
using System.Net.Http;
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

        [Function("WebAppProxyFunction")]
        public async Task<HttpResponseData> Run(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "patch", "delete", "options", Route = "{*any}")] HttpRequestData req,
            FunctionContext context)
        {

            var factory = new WebApplicationFactory<WebApp.Startup>();
            var client = factory.CreateClient();

            var message = new HttpRequestMessage();
            foreach (var header in req.Headers)
            {
                message.Headers.Add(header.Key, header.Value);
            }

            message.Method = new HttpMethod(req.Method);
            message.Content = new StreamContent(req.Body);
            message.RequestUri = req.Url;

            var resultMessage = await client.SendAsync(message);

            var result = req.CreateResponse(resultMessage.StatusCode);
            foreach(var header in resultMessage.Headers) 
            {
                result.Headers.Add(header.Key, header.Value);
            }

            var resultBytes = await resultMessage.Content.ReadAsByteArrayAsync();
            result.WriteBytes(resultBytes);

            return result;
            //return await ProcessRequestAsync(req, executionContext.Logger);
        }
    }
}
