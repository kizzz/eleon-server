using Volo.Abp.Application.Services;

namespace VPortal.HealthCheckModule.Module;

public abstract class IdentityQueryingAppService : ApplicationService
{
  protected IdentityQueryingAppService()
  {
    //LocalizationResource = typeof(HealthCheckModuleResource);
    ObjectMapperContext = typeof(IdentityQueryingApplicationModule);
  }
}
