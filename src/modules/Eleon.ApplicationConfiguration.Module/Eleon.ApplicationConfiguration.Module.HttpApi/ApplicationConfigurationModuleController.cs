using Volo.Abp.AspNetCore.Mvc;
using VPortal.ApplicationConfiguration.Module.Localization;

namespace VPortal.ApplicationConfiguration.Module;

public abstract class ApplicationConfigurationModuleController : AbpControllerBase
{
  protected ApplicationConfigurationModuleController()
  {
    LocalizationResource = typeof(ApplicationConfigurationResource);
  }
}
