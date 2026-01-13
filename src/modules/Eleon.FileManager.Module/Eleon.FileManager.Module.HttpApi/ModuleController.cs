using Volo.Abp.AspNetCore.Mvc;
using VPortal.FileManager.Module.Localization;

namespace VPortal.FileManager.Module;

public abstract class ModuleController : AbpControllerBase
{
  protected ModuleController()
  {
    LocalizationResource = typeof(ModuleResource);
  }
}
