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
using VPortal.TenantManagement.Module;
using VPortal.TenantManagement.Module.Roles;

namespace VPortal.Core.Infrastructure.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Infrastructure/Roles/")]
  public class RoleController : TenantManagementController, IRoleAppService
  {
    private readonly IRoleAppService roleApplicationService;
    private readonly IAbpAuthorizationService abpAuthorizationService;
    private readonly IVportalLogger<RoleController> logger;

    public RoleController(
        IRoleAppService roleApplicationService,
        IAbpAuthorizationService abpAuthorizationService,
        IVportalLogger<RoleController> logger)
    {
      this.roleApplicationService = roleApplicationService;
      this.abpAuthorizationService = abpAuthorizationService;
      this.logger = logger;
    }

    [HttpPost("AddUserToRole")]
    public async Task<bool> AddUserToRole(AddUserToRoleInput input)
    {

      var response = await roleApplicationService.AddUserToRole(input);

      return response;
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

    [HttpPost("RemoveUserFromRole")]
    public async Task<bool> RemoveUserFromRole(RemoveUserFromRoleInput input)
    {

      var response = await roleApplicationService.RemoveUserFromRole(input);

      return response;
    }

    [HttpPost("Create")]
    public async Task<IdentityRoleDto> CreateAsync(IdentityRoleCreateDto input)
    {

      var response = await roleApplicationService.CreateAsync(input);

      return response;
    }

    [HttpDelete("Delete/{id}")]
    public async Task DeleteAsync(Guid id)
    {

      await roleApplicationService.DeleteAsync(id);

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

    [HttpPut("Update/{id}")]
    public async Task<IdentityRoleDto> UpdateAsync(Guid id, IdentityRoleUpdateDto input)
    {

      var response = await roleApplicationService.UpdateAsync(id, input);

      return response;
    }
  }
}
