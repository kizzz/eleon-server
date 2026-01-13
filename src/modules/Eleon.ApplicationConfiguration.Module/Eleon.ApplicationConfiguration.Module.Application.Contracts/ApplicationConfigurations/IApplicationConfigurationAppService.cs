using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Eleon.ApplicationConfiguration.Module.ApplicationConfigurations.Dtos;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.EleoncoreApplicationConfiguration;

namespace Eleon.ApplicationConfiguration.Module.ApplicationConfigurations;

public interface IApplicationConfigurationAppService : IApplicationService
{
  Task<EleoncoreApplicationConfigurationDto> GetAsync(ApplicationConfigurationRequestDto request);
}
