using EleoncoreMultiTenancy.Module.Localization;
using Volo.Abp.Application.Services;
using VPortal.EleoncoreMultiTenancy.Module;

namespace VPortal.TenantManagement.Module;

public abstract class TenantManagementAppService : ApplicationService
{
  protected TenantManagementAppService()
  {
    LocalizationResource = typeof(EleoncoreMultiTenancyResource);
    ObjectMapperContext = typeof(EleoncoreMultiTenancyApplicationModule);
  }
}
