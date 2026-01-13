using EleoncoreMultiTenancy.Module.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace VPortal.TenantManagement.Module;

public abstract class EleoncoreMultiTenancyController : AbpControllerBase
{
  protected EleoncoreMultiTenancyController()
  {
    LocalizationResource = typeof(EleoncoreMultiTenancyResource);
  }
}
