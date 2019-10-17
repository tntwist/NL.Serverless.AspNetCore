using Abp.Application.Services.Dto;

namespace MyCompany.MyProject.Roles.Dto
{
    public class PagedRoleResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

