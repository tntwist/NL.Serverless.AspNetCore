using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace NL.Serverless.AspNetCore.AzureFunctionsHost
{
    public interface IFunctionsRequestHandler
    {
        public Task HandleRequestAsync(HttpRequest httpRequest);
    }
}
