using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace NL.Serverless.AspNetCore.AzureFunctionsHost
{
    internal class FunctionsRequestHandler : IFunctionsRequestHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly RequestDelegate _requestDelegate;

        public FunctionsRequestHandler(
            IServiceProvider serviceProvider,
            RequestDelegate requestDelegate
        )
        {
            _serviceProvider = serviceProvider;
            _requestDelegate = requestDelegate;
        }

        public async Task HandleRequestAsync(HttpRequest httpRequest)
        {
            using (var scope = _serviceProvider.CreateScope()) 
            {
                var context = httpRequest.HttpContext;
                context.RequestServices = scope.ServiceProvider;

                await _requestDelegate(context);
            }
        }
    }
}
