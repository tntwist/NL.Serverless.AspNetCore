using Abp.Authorization;
using MyCompany.MyProject.Authorization.Roles;
using MyCompany.MyProject.Authorization.Users;

namespace MyCompany.MyProject.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
