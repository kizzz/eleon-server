using Volo.Abp.AspNetCore.Mvc;
using VPortal.SystemServicesModule.Module.Localization;

namespace VPortal.SystemServicesModule.Module;

public abstract class SystemServicesModuleController : AbpControllerBase
{
  protected SystemServicesModuleController()
  {
    LocalizationResource = typeof(SystemServicesModuleResource);
  }
}

