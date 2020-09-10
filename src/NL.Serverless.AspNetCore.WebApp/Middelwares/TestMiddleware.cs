using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NL.Serverless.AspNetCore.WebApp.Middelwares
{
    public class TestMiddleware
    {
        private readonly RequestDelegate _next;

        public TestMiddleware(
            RequestDelegate next,
            DiagnosticListener listener
        )
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
