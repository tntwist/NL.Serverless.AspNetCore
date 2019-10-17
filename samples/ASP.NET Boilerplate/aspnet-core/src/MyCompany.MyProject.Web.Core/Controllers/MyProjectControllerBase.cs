using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace MyCompany.MyProject.Controllers
{
    public abstract class MyProjectControllerBase: AbpController
    {
        protected MyProjectControllerBase()
        {
            LocalizationSourceName = MyProjectConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
