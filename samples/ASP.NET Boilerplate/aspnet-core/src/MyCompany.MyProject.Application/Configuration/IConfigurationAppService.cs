using System.Threading.Tasks;
using MyCompany.MyProject.Configuration.Dto;

namespace MyCompany.MyProject.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
