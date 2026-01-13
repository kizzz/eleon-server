using Volo.Abp.AspNetCore.Mvc;
using VPortal.ExternalLink.Module.Localization;

namespace VPortal.ExternalLink.Module;

public abstract class ModuleController : AbpControllerBase
{
  protected ModuleController()
  {
    LocalizationResource = typeof(ModuleResource);
  }
}
