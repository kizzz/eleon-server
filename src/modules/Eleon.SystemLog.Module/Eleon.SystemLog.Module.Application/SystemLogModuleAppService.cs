using Volo.Abp.Application.Services;
using VPortal.DocMessageLog.Module.Localization;

namespace VPortal.DocMessageLog.Module;

public abstract class SystemLogModuleAppService : ApplicationService
{
  protected SystemLogModuleAppService()
  {
    LocalizationResource = typeof(SystemLogResource);
    ObjectMapperContext = typeof(ModuleApplicationModule);
  }
}
