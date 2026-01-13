using Volo.Abp.AspNetCore.Mvc;
using VPortal.HealthCheckModule.Module.Localization;

namespace VPortal.HealthCheckModule.Module;

public abstract class HealthCheckModuleController : AbpControllerBase
{
  protected HealthCheckModuleController()
  {
    LocalizationResource = typeof(HealthCheckModuleResource);
  }
}
