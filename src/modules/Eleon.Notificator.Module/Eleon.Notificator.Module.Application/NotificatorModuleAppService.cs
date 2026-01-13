using Volo.Abp.Application.Services;
using VPortal.Notificator.Module.Localization;

namespace VPortal.Notificator.Module;

public abstract class NotificatorModuleAppService : ApplicationService
{
  protected NotificatorModuleAppService()
  {
    LocalizationResource = typeof(NotificatorResource);
    ObjectMapperContext = typeof(ModuleApplicationModule);
  }
}
