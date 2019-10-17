using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using MyCompany.MyProject.Authorization;

namespace MyCompany.MyProject
{
    [DependsOn(
        typeof(MyProjectCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class MyProjectApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<MyProjectAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(MyProjectApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
