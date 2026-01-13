using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Roles;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Identity;
using VPortal.HealthCheckModule.Module;
using VPortal.TenantManagement.Module;

namespace VPortal.Core.Infrastructure.Module.Controllers
{
  [Area(IdentityQueryingRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = IdentityQueryingRemoteServiceConsts.RemoteServiceName)]
  [Route("api/identity-querying/roles/")]
  public class RoleQueryingController : IdentityQueryingModuleController, IRoleAppService
  {
    private readonly IRoleAppService roleApplicationService;
    private readonly IAbpAuthorizationService abpAuthorizationService;

    public RoleQueryingController(
        IRoleAppService roleApplicationService,
        IAbpAuthorizationService abpAuthorizationService)
    {
      this.roleApplicationService = roleApplicationService;
      this.abpAuthorizationService = abpAuthorizationService;
    }

    [HttpGet("GetList")]
    public async Task<PagedResultDto<CommonRoleDto>> GetListAsync(GetCommonRolesInput input)
    {

      var principal = abpAuthorizationService.CurrentPrincipal;
      var permissions = await abpAuthorizationService.IsGrantedAsync("AbpIdentity.Roles");
      PagedResultDto<CommonRoleDto> response = await roleApplicationService.GetListAsync(input);

      return response;
    }

    [HttpGet("GetUserRolesLookup")]
    public async Task<IList<UserRoleLookupDto>> GetUserRolesLookup(Guid userId)
    {

      var response = await roleApplicationService.GetUserRolesLookup(userId);

      return response;
    }

    [HttpPost("GetUsersInRole")]
    public async Task<PagedResultDto<RoleUserLookupDto>> GetUsersInRole(GetUsersInRoleInput input)
    {

      var response = await roleApplicationService.GetUsersInRole(input);

      return response;
    }

    [HttpGet("IsUserAdmin")]
    public async Task<bool> IsUserAdmin()
    {

      var response = await roleApplicationService.IsUserAdmin();

      return response;
    }

    [HttpGet("GetAllList")]
    public async Task<ListResultDto<IdentityRoleDto>> GetAllListAsync()
    {

      var response = await roleApplicationService.GetAllListAsync();

      return response;
    }

    [HttpGet("Get/{id}")]
    public async Task<IdentityRoleDto> GetAsync(Guid id)
    {

      var response = await roleApplicationService.GetAsync(id);

      return response;
    }
  }
}
