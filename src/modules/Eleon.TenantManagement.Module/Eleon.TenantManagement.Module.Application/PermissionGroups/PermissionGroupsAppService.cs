using Logging.Module;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VPortal.TenantManagement.Module.DomainServices;

namespace VPortal.TenantManagement.Module.PermissionGroups
{
  public class PermissionGroupsAppService : TenantManagementAppService, IPermissionGroupsAppService
  {
    private readonly IVportalLogger<PermissionGroupsAppService> logger;
    private readonly PermissionGroupsDomainService permissionGroupsDomainService;

    public PermissionGroupsAppService(
        IVportalLogger<PermissionGroupsAppService> logger,
        PermissionGroupsDomainService permissionGroupsDomainService)
    {
      this.logger = logger;
      this.permissionGroupsDomainService = permissionGroupsDomainService;
    }

    public async Task<List<PermissionGroupCategory>> GetPermissionGroups()
    {
      List<PermissionGroupCategory> result = null;
      try
      {
        result = await permissionGroupsDomainService.GetPermissionGroupCategories();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
