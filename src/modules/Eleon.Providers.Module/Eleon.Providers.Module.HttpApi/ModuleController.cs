using Volo.Abp.AspNetCore.Mvc;
using VPortal.Storage.Module.Localization;

namespace VPortal.Storage.Module;

public abstract class ModuleController : AbpControllerBase
{
  protected ModuleController()
  {
    LocalizationResource = typeof(StorageModuleResource);
  }
}
