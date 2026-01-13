using Volo.Abp.AspNetCore.Mvc;
//using VPortal.Storage.Module.Localization;

namespace VPortal.Storage.Module;

public abstract class StorageModuleController : AbpControllerBase
{
  protected StorageModuleController()
  {
    //LocalizationResource = typeof(StorageModuleResource);
  }
}
