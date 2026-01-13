using Volo.Abp.AspNetCore.Mvc;
using VPortal.BackgroundJobs.Module.Localization;

namespace VPortal.BackgroundJobs.Module;

public abstract class ModuleController : AbpControllerBase
{
  protected ModuleController()
  {
    LocalizationResource = typeof(ModuleResource);
  }
}
