using Volo.Abp.AspNetCore.Mvc;

namespace VPortal.HealthCheckModule.Module;

public abstract class IdentityQueryingModuleController : AbpControllerBase
{
  protected IdentityQueryingModuleController()
  {
    // LocalizationResource = typeof(IdentityQueryingModuleResource);
  }
}
