using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace MyCompany.MyProject.Web.FunctionApp
{
    public class FunctionAppHostingEnvironment : IWebHostEnvironment
    {
        public IFileProvider WebRootFileProvider { get; set; }
        public string WebRootPath { get; set; }
        public string ApplicationName { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public string EnvironmentName { get; set; }
    }
}
