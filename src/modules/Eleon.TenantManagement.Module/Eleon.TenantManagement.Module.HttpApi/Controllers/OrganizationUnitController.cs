using Common.Module.Constants;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.TenantManagement.Module;
using VPortal.TenantManagement.Module.OrganizationUnits;
using VPortal.TenantManagement.Module.Roles;
using VPortal.TenantManagement.Module.Users;

namespace Core.Infrastructure.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Infrastructure/OrganizationUnit/")]
  public class OrganizationUnitController : TenantManagementController, IOrganizationUnitAppService
  {
    private readonly IOrganizationUnitAppService appService;
    private readonly IVportalLogger<OrganizationUnitController> logger;

    public OrganizationUnitController(
        IOrganizationUnitAppService appService,
        IVportalLogger<OrganizationUnitController> logger)
    {
      this.appService = appService;
      this.logger = logger;
    }

    [HttpPost("AddMember")]
    public async Task AddMemberAsync(Guid userId, Guid orgUnitId)
    {

      await appService.AddMemberAsync(userId, orgUnitId);

    }

    [HttpPost("AddRole")]
    public async Task AddRoleAsync(Guid roleId, Guid orgUnitId)
    {

      await appService.AddRoleAsync(roleId, orgUnitId);

    }

    [HttpPost("CreateAsync")]
    public async Task<CommonOrganizationUnitDto> CreateAsync(CommonOrganizationUnitDto orgUnit)
    {

      var response = await appService.CreateAsync(orgUnit);


      return response;
    }

    [HttpDelete("DeleteAsync")]
    public async Task DeleteAsync(Guid orgUnitId)
    {

      await appService.DeleteAsync(orgUnitId);

    }

    [HttpPost("UpdateAsync")]
    public async Task<CommonOrganizationUnitDto> UpdateAsync(CommonOrganizationUnitDto orgUnit)
    {

      var response = await appService.UpdateAsync(orgUnit);


      return response;
    }

    [HttpGet("GetAvailableForUser")]
    public async Task<List<UserOrganizationUnitLookupDto>> GetAvailableForUserAsync(GetAvatilableOrgUnitsInput input)
    {

      var response = await appService.GetAvailableForUserAsync(input);


      return response;
    }

    [HttpGet("GetById")]
    public async Task<CommonOrganizationUnitDto> GetByIdAsync(Guid id, bool includeSoftDeleted = false)
    {

      var response = await appService.GetByIdAsync(id, includeSoftDeleted);


      return response;
    }

    [HttpGet("GetListAsync")]
    public async Task<List<CommonOrganizationUnitDto>> GetListAsync()
    {

      var response = await appService.GetListAsync();


      return response;
    }

    [HttpGet("GetMembers")]
    public async Task<List<CommonUserDto>> GetMembers(CommonOrganizationUnitDto orgUnit)
    {

      var response = await appService.GetMembers(orgUnit);


      return response;
    }

    [HttpGet("GetRoles")]
    public async Task<List<CommonRoleDto>> GetRoles(CommonOrganizationUnitDto orgUnit)
    {

      var response = await appService.GetRoles(orgUnit);


      return response;
    }

    [HttpDelete("RemoveMember")]
    public async Task RemoveMember(Guid userId, Guid orgUnitId)
    {

      await appService.RemoveMember(userId, orgUnitId);

    }

    [HttpDelete("RemoveRole")]
    public async Task RemoveRole(Guid roleId, Guid orgUnitId)
    {

      await appService.RemoveRole(roleId, orgUnitId);

    }

    [HttpPost("CloneAsync")]
    public async Task<CommonOrganizationUnitTreeNodeDto> Clone(Guid id, string newName, bool withRoles, bool withMembers, bool withChildren)
    {

      var response = await appService.Clone(id, newName, withRoles, withMembers, withChildren);


      return response;
    }

    [HttpPost("MoveAsync")]
    public async Task<bool> Move(Guid id, Guid newParentId)
    {

      var response = await appService.Move(id, newParentId);


      return response;
    }

    [HttpGet("GetUserOrganizationUnits")]
    public async Task<List<CommonOrganizationUnitDto>> GetUserOrganizationUnits(Guid userId)
    {

      var response = await appService.GetUserOrganizationUnits(userId);


      return response;
    }

    [HttpPost("SetUserOrganizationUnits")]
    public async Task<bool> SetUserOrganizationUnits(SetUserOrganizationUnitsInput input)
    {

      var response = await appService.SetUserOrganizationUnits(input);


      return response;
    }

    [HttpGet("GetRoleOrganizationUnits")]
    public async Task<List<CommonOrganizationUnitDto>> GetRoleOrganizationUnits(string roleName)
    {

      var response = await appService.GetRoleOrganizationUnits(roleName);


      return response;
    }

    [HttpPost("SetRoleOrganizationUnits")]
    public async Task<bool> SetRoleOrganizationUnits(SetRoleOrganizationUnitsInput input)
    {

      var response = await appService.SetRoleOrganizationUnits(input);


      return response;
    }

    [HttpGet("GetHighLevelOrgUnitListAsync")]
    public async Task<List<CommonOrganizationUnitDto>> GetHighLevelOrgUnitListAsync()
    {

      var response = await appService.GetHighLevelOrgUnitListAsync();


      return response;
    }

    [HttpGet("CheckOrganizationUnitPermission")]
    public async Task<bool> CheckOrganizationUnitPermissionAsync(Guid orgUnitId, string permission)
    {

      var response = await appService.CheckOrganizationUnitPermissionAsync(orgUnitId, permission);


      return response;
    }

    [HttpGet("CheckOrganizationUnitParentsPermission")]
    public async Task<bool> CheckOrganizationUnitParentsPermissionAsync(Guid orgUnitId, string permission)
    {

      var response = await appService.CheckOrganizationUnitParentsPermissionAsync(orgUnitId, permission);


      return response;
    }

    [HttpGet("GetDocumentSeriaNumber")]
    public async Task<string> GetDocumentSeriaNumberAsync(string documentObjectType, string prefix, string refId)
    {

      var response = await appService.GetDocumentSeriaNumberAsync(documentObjectType, prefix, refId);


      return response;
    }

    [HttpPost("GetAllUnitAndChildsMembers")]
    public async Task<PagedResultDto<CommonUserDto>> GetAllUnitAndChildsMembersAsync(GetAllUnitAndChildsMembersRequestDto input)
    {

      var response = await appService.GetAllUnitAndChildsMembersAsync(input);


      return response;
    }

    [HttpPost("AddMembers")]
    public Task AddMembersAsync(Guid orgUnitId, List<Guid> userIds)
    {

      var response = appService.AddMembersAsync(orgUnitId, userIds);


      return response;
    }

    [HttpPost("AddRoles")]
    public Task AddRolesAsync(Guid orgUnitId, List<Guid> roleIds)
    {

      var response = appService.AddRolesAsync(orgUnitId, roleIds);


      return response;
    }
  }
}
