using Volo.Abp.AspNetCore.Mvc;
using VPortal.Auditor.Module.Localization;

namespace VPortal.Auditor.Module;

public abstract class ModuleController : AbpControllerBase
{
  protected ModuleController()
  {
    LocalizationResource = typeof(ModuleResource);
  }
}
