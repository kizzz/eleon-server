using Volo.Abp.Application.Services;
using VPortal.Storage.Module.Localization;

namespace VPortal.Storage.Remote.Application;

public abstract class StorageRemoteModuleAppService : ApplicationService
{
  protected StorageRemoteModuleAppService()
  {
    LocalizationResource = typeof(StorageModuleResource);
    ObjectMapperContext = typeof(StorageRemoteApplicationModule);
  }
}
