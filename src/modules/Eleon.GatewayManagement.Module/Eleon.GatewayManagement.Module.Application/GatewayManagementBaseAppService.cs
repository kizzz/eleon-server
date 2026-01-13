using Volo.Abp.Application.Services;
using VPortal.GatewayManagement.Module.Localization;

namespace VPortal.GatewayManagement.Module;

public abstract class GatewayManagementBaseAppService : ApplicationService
{
  protected GatewayManagementBaseAppService()
  {
    LocalizationResource = typeof(GatewayManagementResource);
    ObjectMapperContext = typeof(GatewayManagementApplicationModule);
  }
}
