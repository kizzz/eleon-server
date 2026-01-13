using Volo.Abp.AspNetCore.Mvc;
using VPortal.SitesManagement.Module.Localization;

namespace VPortal.SitesManagement.Module;

public abstract class SitesManagementController : AbpControllerBase
{
  protected SitesManagementController()
  {
    LocalizationResource = typeof(SitesManagementResource);
  }
}

