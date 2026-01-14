using Volo.Abp.Application.Services;
using VPortal.SystemServicesModule.Module.Localization;

namespace VPortal.SystemServicesModule.Module;

public abstract class SystemServicesModuleAppService : ApplicationService
{
  protected SystemServicesModuleAppService()
  {
    LocalizationResource = typeof(SystemServicesModuleResource);
    ObjectMapperContext = typeof(SystemServicesApplicationModule);
  }
}

