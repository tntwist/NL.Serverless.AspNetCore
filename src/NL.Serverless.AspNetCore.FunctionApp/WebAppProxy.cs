using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting;

namespace NL.Serverless.AspNetCore.FunctionApp
{
    /// <summary>
    /// Proxy function to delegate incoming http request to web app.
    /// </summary>
    public class WebAppProxy
    {
        private readonly IWebHost _server;
        private readonly RequestDelegate _requestDelegate;

        //private readonly RequestDelegate _requestDelegate;
        //private readonly WebAppServiceProvider _serviceProvider;

        //public WebAppProxy(RequestDelegate requestDelegate, WebAppServiceProvider serviceProvider)
        //{
        //    _requestDelegate = requestDelegate;
        //    _serviceProvider = serviceProvider;
        //}

        public WebAppProxy(IWebHost server, RequestDelegate requestDelegate)
        {
            _server = server;
            _requestDelegate = requestDelegate;
        }

        [FunctionName("Function1")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "patch", "delete",  Route = "{*any}")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("WebAppProxy processed a request.");

            try
            {
                using (var scope = _server.Services.CreateScope())
                {
                    req.HttpContext.RequestServices = scope.ServiceProvider;
                    await _requestDelegate(req.HttpContext);
                }

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
