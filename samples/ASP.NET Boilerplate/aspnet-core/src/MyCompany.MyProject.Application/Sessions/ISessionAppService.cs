using System.Threading.Tasks;
using Abp.Application.Services;
using MyCompany.MyProject.Sessions.Dto;

namespace MyCompany.MyProject.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
