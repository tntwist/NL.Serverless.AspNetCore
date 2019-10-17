using Microsoft.AspNetCore.Antiforgery;
using MyCompany.MyProject.Controllers;

namespace MyCompany.MyProject.Web.Host.Controllers
{
    public class AntiForgeryController : MyProjectControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
