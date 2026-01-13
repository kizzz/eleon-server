using Volo.Abp.AspNetCore.Mvc;
using VPortal.Lifecycle.Feature.Module.Localization;

namespace VPortal.Lifecycle.Feature.Module;

public abstract class ModuleController : AbpControllerBase
{
  protected ModuleController()
  {
    LocalizationResource = typeof(LifecycleFeatureModuleResource);
  }
}
