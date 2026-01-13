using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.TenantManagement.Module.PermissionGroups;

namespace VPortal.TenantManagement.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/TenantManagement/PermissionGroups")]
  public class PermissionGroupController : TenantManagementController, IPermissionGroupsAppService
  {
    private readonly IVportalLogger<PermissionGroupController> logger;
    private readonly IPermissionGroupsAppService appService;

    public PermissionGroupController(
        IVportalLogger<PermissionGroupController> logger,
        IPermissionGroupsAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }

    [HttpGet("GetPermissionGroups")]
    public async Task<List<PermissionGroupCategory>> GetPermissionGroups()
    {

      var response = await appService.GetPermissionGroups();


      return response;
    }
  }
}
