using Volo.Abp.Application.Services;
//using VPortal.Storage.Module.Localization;

namespace VPortal.Storage.Module;

public abstract class StorageModuleAppService : ApplicationService
{
  protected StorageModuleAppService()
  {
    //LocalizationResource = typeof(StorageModuleResource);
    ObjectMapperContext = typeof(StorageApplicationModule);
  }
}
