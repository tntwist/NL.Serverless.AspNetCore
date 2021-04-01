using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NL.Serverless.AspNetCore.AzureFunctionsHost
{
    internal class FunctionsRequestHandler : IFunctionsRequestHandler
    {
        private readonly HttpClient _httpClient;

        public FunctionsRequestHandler(
            HttpClient httpClient
        )
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseData> HandleRequestAsync(HttpRequestData httpRequest)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = new HttpMethod(httpRequest.Method),
                Content = new StreamContent(httpRequest.Body),
                RequestUri = httpRequest.Url
            };

            // add non content headers to request message
            httpRequest.Headers.Where(x => !x.Key.Contains("Content", StringComparison.OrdinalIgnoreCase))
                .ToList()
                .ForEach(header => requestMessage.Headers.Add(header.Key, header.Value));

            // add content headers to request content
            httpRequest.Headers.Where(x => x.Key.Contains("Content", StringComparison.OrdinalIgnoreCase))
                .ToList()
                .ForEach(header => requestMessage.Content.Headers.Add(header.Key, header.Value));

            var reponseMessage = await _httpClient.SendAsync(requestMessage);

            var result = httpRequest.CreateResponse(reponseMessage.StatusCode);

            var resultBytes = await reponseMessage.Content.ReadAsByteArrayAsync();
            result.WriteBytes(resultBytes);

            foreach (var header in reponseMessage.Content.Headers)
            {
                result.Headers.Add(header.Key, header.Value);
            }
            foreach (var header in reponseMessage.Headers)
            {
                result.Headers.Add(header.Key, header.Value);
            }

            return result;
        }
    }
}
