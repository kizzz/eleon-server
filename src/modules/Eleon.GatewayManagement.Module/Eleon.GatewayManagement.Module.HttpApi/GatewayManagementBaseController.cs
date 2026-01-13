using Volo.Abp.AspNetCore.Mvc;
using VPortal.GatewayManagement.Module.Localization;

namespace VPortal.GatewayManagement.Module;

public abstract class GatewayManagementBaseController : AbpControllerBase
{
  protected GatewayManagementBaseController()
  {
    LocalizationResource = typeof(GatewayManagementResource);
  }
}
