using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using NL.Serverless.AspNetCore.AzureFunctionsHost;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NL.Serverless.AspNetCore.FunctionApp
{
    /// <summary>
    /// Proxy function to delegate incoming http request to web app.
    /// </summary>
    public class WebAppProxyFunction : WebAppProxyFunctionBase
    {
        private static HttpClient _client;

        public WebAppProxyFunction(IFunctionsRequestHandler requestHandler) : base(requestHandler)
        {
        }

        [Function("WebAppProxyFunction")]
        public async Task<HttpResponseData> Run(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "patch", "delete", "options", Route = "{*any}")] HttpRequestData req,
            FunctionContext context)
        {
            if(_client == null) 
            {
                var factory = new WebApplicationFactory<WebApp.Startup>();
                _client = factory.CreateClient(new WebApplicationFactoryClientOptions 
                {
                    AllowAutoRedirect = false
                });
            }

            var requestMessage = new HttpRequestMessage
            {
                Method = new HttpMethod(req.Method),
                Content = new StreamContent(req.Body),
                RequestUri = req.Url
            };

            // add non content headers to request message
            req.Headers.Where(x => !x.Key.Contains("Content", System.StringComparison.OrdinalIgnoreCase))
                .ToList()
                .ForEach(header => requestMessage.Headers.Add(header.Key, header.Value));

            // add content headers to request content
            req.Headers.Where(x => x.Key.Contains("Content", System.StringComparison.OrdinalIgnoreCase))
                .ToList()
                .ForEach(header => requestMessage.Content.Headers.Add(header.Key, header.Value));

            var reponseMessage = await _client.SendAsync(requestMessage);

            var result = req.CreateResponse(reponseMessage.StatusCode);

            var resultBytes = await reponseMessage.Content.ReadAsByteArrayAsync();
            result.WriteBytes(resultBytes);

            foreach(var header in reponseMessage.Content.Headers) 
            {
                result.Headers.Add(header.Key, header.Value);
            }
            foreach (var header in reponseMessage.Headers)
            {
                result.Headers.Add(header.Key, header.Value);
            }

            return result;
            //return await ProcessRequestAsync(req, executionContext.Logger);
        }
    }
}
