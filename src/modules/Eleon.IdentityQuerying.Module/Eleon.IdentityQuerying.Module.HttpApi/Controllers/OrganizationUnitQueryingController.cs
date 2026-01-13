using Common.Module.Constants;
using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.OrganizationUnits;
using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Roles;
using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Users;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.HealthCheckModule.Module;
using VPortal.TenantManagement.Module;

namespace Core.Infrastructure.Module.Controllers
{
  [Area(IdentityQueryingRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = IdentityQueryingRemoteServiceConsts.RemoteServiceName)]
  [Route("api/identity-querying/organization-unit/")]
  public class OrganizationUnitQueryingController : IdentityQueryingModuleController, IOrganizationUnitAppService
  {
    private readonly IOrganizationUnitAppService appService;

    public OrganizationUnitQueryingController(IOrganizationUnitAppService appService)
    {
      this.appService = appService;
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

    [HttpGet("GetUserOrganizationUnits")]
    public async Task<List<CommonOrganizationUnitDto>> GetUserOrganizationUnits(Guid userId)
    {

      var response = await appService.GetUserOrganizationUnits(userId);


      return response;
    }

    [HttpGet("GetRoleOrganizationUnits")]
    public async Task<List<CommonOrganizationUnitDto>> GetRoleOrganizationUnits(string roleName)
    {

      var response = await appService.GetRoleOrganizationUnits(roleName);


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
  }
}
