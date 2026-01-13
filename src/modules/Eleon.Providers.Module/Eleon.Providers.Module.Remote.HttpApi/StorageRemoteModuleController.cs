using Volo.Abp.AspNetCore.Mvc;
using VPortal.Storage.Module.Localization;

namespace VPortal.Storage.Remote.HttpApi;

public abstract class StorageRemoteModuleController : AbpControllerBase
{
  protected StorageRemoteModuleController()
  {
    LocalizationResource = typeof(StorageModuleResource);
  }
}
