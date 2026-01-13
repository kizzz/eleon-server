using Volo.Abp.AspNetCore.Mvc;
using VPortal.EventManagementModule.Module.Localization;

namespace VPortal.EventManagementModule.Module;

public abstract class EventManagementCotroller : AbpControllerBase
{
  protected EventManagementCotroller()
  {
    LocalizationResource = typeof(EventManagementModuleResource);
  }
}
