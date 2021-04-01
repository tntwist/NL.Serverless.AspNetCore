using Microsoft.Azure.Functions.Worker.Http;
using System.Threading.Tasks;

namespace NL.Serverless.AspNetCore.AzureFunctionsHost
{
    public interface IFunctionsRequestHandler
    {
        public Task<HttpResponseData> HandleRequestAsync(HttpRequestData httpRequest);
    }
}
