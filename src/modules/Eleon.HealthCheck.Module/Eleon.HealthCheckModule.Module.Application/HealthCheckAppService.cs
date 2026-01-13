using Volo.Abp.Application.Services;
using VPortal.HealthCheckModule.Module.Localization;

namespace VPortal.HealthCheckModule.Module;

public abstract class HealthCheckModuleAppService : ApplicationService
{
  protected HealthCheckModuleAppService()
  {
    LocalizationResource = typeof(HealthCheckModuleResource);
    ObjectMapperContext = typeof(HealthCheckApplicationModule);
  }
}
