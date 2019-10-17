using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using MyCompany.MyProject.MultiTenancy;

namespace MyCompany.MyProject.Sessions.Dto
{
    [AutoMapFrom(typeof(Tenant))]
    public class TenantLoginInfoDto : EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }
    }
}
