using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MyCompany.MyProject.Configuration;
using MyCompany.MyProject.Web;

namespace MyCompany.MyProject.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class MyProjectDbContextFactory : IDesignTimeDbContextFactory<MyProjectDbContext>
    {
        public MyProjectDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<MyProjectDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            MyProjectDbContextConfigurer.Configure(builder, configuration.GetConnectionString(MyProjectConsts.ConnectionStringName));

            return new MyProjectDbContext(builder.Options);
        }
    }
}
