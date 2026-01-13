using Volo.Abp.Application.Services;
using VPortal.ApplicationConfiguration.Module.Localization;

namespace VPortal.ApplicationConfiguration.Module;

public abstract class ApplicationConfigurationModuleAppService : ApplicationService
{
  protected ApplicationConfigurationModuleAppService()
  {
    LocalizationResource = typeof(ApplicationConfigurationResource);
    ObjectMapperContext = typeof(ApplicationConfigurationApplicationModule);
  }
}
