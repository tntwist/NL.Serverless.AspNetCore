using System;

namespace MyCompany.MyProject.Web.FunctionApp
{
    /// <summary>
    /// Holds the service provider for the request proccessed by the web app.
    /// </summary>
    public class WebAppServiceProvider
    {
        public IServiceProvider ServiceProvider { get; set; }
    }
}
