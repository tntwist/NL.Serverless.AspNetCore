using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using MyCompany.MyProject.Authorization.Roles;
using MyCompany.MyProject.Authorization.Users;
using MyCompany.MyProject.MultiTenancy;

namespace MyCompany.MyProject.EntityFrameworkCore
{
    public class MyProjectDbContext : AbpZeroDbContext<Tenant, Role, User, MyProjectDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public MyProjectDbContext(DbContextOptions<MyProjectDbContext> options)
            : base(options)
        {
        }
    }
}
