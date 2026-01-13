using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.PermissionManagement;

namespace VPortal.TenantManagement.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/TenantManagement/Permissions")]
  public class PermissionsController : TenantManagementController, IPermissionAppService
  {
    private readonly IVportalLogger<PermissionsController> logger;
    private readonly IPermissionAppService permissionAppService;

    public PermissionsController(
        IVportalLogger<PermissionsController> logger,
        IPermissionAppService permissionAppService)
    {
      this.logger = logger;
      this.permissionAppService = permissionAppService;
    }

    [HttpGet("GetAsync")]
    public async Task<GetPermissionListResultDto> GetAsync(string providerName, string providerKey)
    {

      var response = await permissionAppService.GetAsync(providerName, providerKey);

      return response;
    }

    [HttpGet("GetByGroupAsync")]
    public Task<GetPermissionListResultDto> GetByGroupAsync(string groupName, string providerName, string providerKey)
    {

      var resp = permissionAppService.GetByGroupAsync(groupName, providerName, providerKey);


      return resp;
    }

    [HttpPost("UpdateAsync")]
    public async Task UpdateAsync(string providerName, string providerKey, UpdatePermissionsDto input)
    {

      await permissionAppService.UpdateAsync(providerName, providerKey, input);

    }
  }
}
