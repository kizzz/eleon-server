using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using VPortal.TenantManagement.Module.DomainServices;
using VPortal.TenantManagement.Module.Permissions;

namespace VPortal.TenantManagement.Module.TenantStatuses
{
  [Authorize]
  public class TenantStatusAppService : TenantManagementAppService, ITenantStatusAppService
  {
    private readonly IVportalLogger<TenantStatusAppService> logger;
    private readonly VportalPermissionHelper permissionHelper;
    private readonly TenantStatusDomainService domainService;

    public TenantStatusAppService(
        IVportalLogger<TenantStatusAppService> logger,
        VportalPermissionHelper permissionHelper,
        TenantStatusDomainService domainService)
    {
      this.logger = logger;
      this.permissionHelper = permissionHelper;
      this.domainService = domainService;
    }

    public async Task<bool> SuspendTenant(Guid tenantId)
    {
      bool result = false;
      try
      {
        await permissionHelper.EnsureHostAdmin();
        await domainService.SuspendTenantWithReplication(tenantId);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> UnsuspendTenant(Guid tenantId)
    {
      bool result = false;
      try
      {
        await permissionHelper.EnsureHostAdmin();
        await domainService.ActivateTenantWithReplication(tenantId);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
