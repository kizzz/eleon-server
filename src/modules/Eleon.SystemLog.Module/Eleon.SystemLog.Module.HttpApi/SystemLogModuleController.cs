using Volo.Abp.AspNetCore.Mvc;
using VPortal.DocMessageLog.Module.Localization;

namespace VPortal.DocMessageLog.Module;

public abstract class SystemLogModuleController : AbpControllerBase
{
  protected SystemLogModuleController()
  {
    LocalizationResource = typeof(SystemLogResource);
  }
}
