using Volo.Abp.Application.Services;
using VPortal.EventManagementModule.Module.Localization;

namespace VPortal.EventManagementModule.Module;

public abstract class EventManagementAppService : ApplicationService
{
  protected EventManagementAppService()
  {
    LocalizationResource = typeof(EventManagementModuleResource);
    ObjectMapperContext = typeof(EventManagementApplicationModule);
  }
}
