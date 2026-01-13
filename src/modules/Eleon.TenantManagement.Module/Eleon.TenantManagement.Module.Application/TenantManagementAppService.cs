using Volo.Abp.Application.Services;
using VPortal.TenantManagement.Module.Localization;

namespace VPortal.TenantManagement.Module;

public abstract class TenantManagementAppService : ApplicationService
{
  protected TenantManagementAppService()
  {
    LocalizationResource = typeof(TenantManagementResource);
    ObjectMapperContext = typeof(TenantManagementApplicationModule);
  }
}
