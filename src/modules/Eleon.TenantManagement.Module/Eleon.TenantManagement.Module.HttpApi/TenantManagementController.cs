using Volo.Abp.AspNetCore.Mvc;
using VPortal.TenantManagement.Module.Localization;

namespace VPortal.TenantManagement.Module;

public abstract class TenantManagementController : AbpControllerBase
{
  protected TenantManagementController()
  {
    LocalizationResource = typeof(TenantManagementResource);
  }
}
