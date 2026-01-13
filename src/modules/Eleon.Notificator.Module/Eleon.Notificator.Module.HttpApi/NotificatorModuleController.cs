using Volo.Abp.AspNetCore.Mvc;
using VPortal.Notificator.Module.Localization;

namespace VPortal.Notificator.Module;

public abstract class NotificatorModuleController : AbpControllerBase
{
  protected NotificatorModuleController()
  {
    LocalizationResource = typeof(NotificatorResource);
  }
}
