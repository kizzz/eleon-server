using Common.EventBus.Module;
using Common.Module.ValueObjects;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using VPortal.TenantManagement.Module;
using VPortal.TenantManagement.Module.DomainServices;
using VPortal.TenantManagement.Module.OrganizationUnits;
using VPortal.TenantManagement.Module.Roles;
using VPortal.TenantManagement.Module.Users;

namespace Core.Infrastructure.Module.OrganizationUnits.Application
{
  public class OrganizationUnitAppService : TenantManagementAppService, IOrganizationUnitAppService
  {
    private readonly IVportalLogger<OrganizationUnitAppService> logger;
    private readonly OrganizationUnitDomainService domainService;
    private readonly RoleDomainService roleDomainService;
    private readonly IDistributedEventBus _eventBus;

    public OrganizationUnitAppService(
        IVportalLogger<OrganizationUnitAppService> logger,
        OrganizationUnitDomainService domainService,
        RoleDomainService roleDomainService,
        IDistributedEventBus eventBus)
    {
      this.logger = logger;
      this.domainService = domainService;
      this.roleDomainService = roleDomainService;
      _eventBus = eventBus;
    }

    public async Task<CommonOrganizationUnitDto> CreateAsync(CommonOrganizationUnitDto input)
    {
      CommonOrganizationUnitDto result = null;
      try
      {
        var orgUnit = new OrganizationUnit(GuidGenerator.Create(), input.DisplayName, input.ParentId, CurrentTenant.Id);
        orgUnit.ExtraProperties["Currency"] = input.Code;

        orgUnit = await domainService.CreateAsync(orgUnit);

        result = ObjectMapper.Map<OrganizationUnit, CommonOrganizationUnitDto>(orgUnit);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task AddMemberAsync(Guid userId, Guid orgUnitId)
    {
      try
      {
        await domainService.AddMemberAsync(userId, orgUnitId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }

    public async Task AddMembersAsync(Guid orgUnitId, List<Guid> userIds)
    {
      try
      {
        foreach (var userId in userIds)
        {
          await domainService.AddMemberAsync(userId, orgUnitId);
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }


    public async Task RemoveMember(Guid userId, Guid orgUnitId)
    {
      try
      {
        await domainService.RemoveMember(userId, orgUnitId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }

    public async Task AddRoleAsync(Guid roleId, Guid orgUnitId)
    {
      try
      {
        await domainService.AddRoleAsync(roleId, orgUnitId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }

    public async Task AddRolesAsync(Guid orgUnitId, List<Guid> roleIds)
    {
      try
      {
        foreach (var roleId in roleIds)
        {
          await domainService.AddRoleAsync(roleId, orgUnitId);
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }


    public async Task RemoveRole(Guid roleId, Guid orgUnitId)
    {
      try
      {
        await domainService.RemoveRole(roleId, orgUnitId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }

    public async Task DeleteAsync(Guid orgUnitId)
    {
      try
      {
        await domainService.DeleteAsync(orgUnitId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }

    public async Task<CommonOrganizationUnitDto> UpdateAsync(CommonOrganizationUnitDto input)
    {
      CommonOrganizationUnitDto result = null;
      try
      {
        var orgUnit = ObjectMapper.Map<CommonOrganizationUnitDto, OrganizationUnit>(input);
        orgUnit.ExtraProperties["IsEnabled"] = input.IsEnabled;
        orgUnit = await domainService.UpdateAsync(orgUnit);

        result = ObjectMapper.Map<OrganizationUnit, CommonOrganizationUnitDto>(orgUnit);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<List<UserOrganizationUnitLookupDto>> GetAvailableForUserAsync(GetAvatilableOrgUnitsInput input)
    {
      List<UserOrganizationUnitLookupDto> result = null;
      try
      {
        var userId = input.UserId == Guid.Empty || input.UserId == null
            ? CurrentUser.Id.Value
            : input.UserId.Value;
        var entities = await domainService.GetAvailableForUser(userId);
        result = ObjectMapper.Map<List<UserOrganizationUnitLookup>, List<UserOrganizationUnitLookupDto>>(entities);
        result.ForEach(x => x.OrganizationUnit.IsEnabled = true);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<CommonOrganizationUnitDto> GetByIdAsync(Guid id, bool includeSoftDeleted = false)
    {
      CommonOrganizationUnitDto result = null;
      try
      {
        var entity = await domainService.GetByIdAsync(id, includeSoftDeleted);
        result = ObjectMapper.Map<OrganizationUnit, CommonOrganizationUnitDto>(entity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<List<CommonOrganizationUnitDto>> GetListAsync()
    {
      List<CommonOrganizationUnitDto> result = null;
      try
      {
        var entities = await domainService.GetListAsync();
        result = ObjectMapper.Map<List<OrganizationUnit>, List<CommonOrganizationUnitDto>>(entities);

        if (result != null && result.Count > 0)
        {
          foreach (var unit in result)
          {
            var unitEntity = entities.First(u => u.Id == unit.Id);

            if (unitEntity.ExtraProperties.TryGetValue("IsEnabled", out object isEnabledObject))
            {
              unit.IsEnabled = Convert.ToBoolean(isEnabledObject);
            }
          }

        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<List<CommonUserDto>> GetMembers(CommonOrganizationUnitDto orgUnitDto)
    {
      List<CommonUserDto> result = null;
      try
      {
        var orgUnit = ObjectMapper.Map<CommonOrganizationUnitDto, OrganizationUnit>(orgUnitDto);
        var identityUsers = await domainService.GetMembers(orgUnit);
        result = ObjectMapper.Map<List<IdentityUser>, List<CommonUserDto>>(identityUsers);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<List<CommonRoleDto>> GetRoles(CommonOrganizationUnitDto orgUnitDto)
    {
      List<CommonRoleDto> result = null;
      try
      {
        var orgUnit = ObjectMapper.Map<CommonOrganizationUnitDto, OrganizationUnit>(orgUnitDto);
        var identityRoles = await domainService.GetRoles(orgUnit);
        result = ObjectMapper.Map<List<CommonRoleValueObject>, List<CommonRoleDto>>(identityRoles);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<CommonOrganizationUnitTreeNodeDto> Clone(Guid id, string newName, bool withRoles, bool withMembers, bool withChildren)
    {

      CommonOrganizationUnitTreeNodeDto result = null;
      try
      {
        var entity = await domainService.CloneAsync(id, newName, withRoles, withMembers, withChildren);
        result = ObjectMapper.Map<TreeNodeValueObject<OrganizationUnit>, CommonOrganizationUnitTreeNodeDto>(entity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;

    }

    public async Task<bool> Move(Guid id, Guid newParentId)
    {
      bool result = false;
      try
      {
        result = await domainService.Move(id, newParentId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;
    }

    [Authorize]
    public async Task<List<CommonOrganizationUnitDto>> GetUserOrganizationUnits(Guid userId)
    {
      List<CommonOrganizationUnitDto> result = null;
      try
      {
        var ous = await domainService.GetUserOrganizationUnits(userId);
        result = ObjectMapper.Map<List<OrganizationUnit>, List<CommonOrganizationUnitDto>>(ous);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<bool> SetUserOrganizationUnits(SetUserOrganizationUnitsInput input)
    {
      bool result = false;
      try
      {
        await domainService.SetUserOrganizationUnits(input.UserId, input.OrganizationUnitIds);
        result = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<List<CommonOrganizationUnitDto>> GetRoleOrganizationUnits(string roleName)
    {
      List<CommonOrganizationUnitDto> result = null;
      try
      {
        var ous = await domainService.GetRoleOrganizationUnits(roleName);
        result = ObjectMapper.Map<List<OrganizationUnit>, List<CommonOrganizationUnitDto>>(ous);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<bool> SetRoleOrganizationUnits(SetRoleOrganizationUnitsInput input)
    {
      bool result = false;
      try
      {
        await domainService.SetRoleOrganizationUnits(input.RoleName, input.OrganizationUnitIds);
        result = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<bool> CheckIfMemberExist(List<CommonUserDto> members)
    {
      bool result = false;
      try
      {
        var entities = ObjectMapper.Map<List<CommonUserDto>, List<IdentityUser>>(members);
        result = await domainService.CheckIfMemberExist(entities);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<List<CommonOrganizationUnitDto>> GetHighLevelOrgUnitListAsync()
    {
      List<CommonOrganizationUnitDto> result = null;
      try
      {
        var entities = await domainService.GetListAsync();
        var units = entities?.Where(x => !x.ParentId.HasValue).ToList();
        result = ObjectMapper.Map<List<OrganizationUnit>, List<CommonOrganizationUnitDto>>(units);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> CheckOrganizationUnitPermissionAsync(Guid orgUnitId, string permission)
    {
      bool result = false;

      try
      {
        result = await domainService.CheckPermissionAsync(orgUnitId, permission);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> CheckOrganizationUnitParentsPermissionAsync(Guid orgUnitId, string permission)
    {
      bool result = false;

      try
      {
        result = await domainService.CheckParentsPermissionAsync(orgUnitId, permission);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<string> GetDocumentSeriaNumberAsync(string documentObjectType, string prefix, string refId)
    {
      string result = null;

      try
      {
        var response = await _eventBus.RequestAsync<DocumentSeriaNumberGotMsg>(new GetDocumentSeriaNumberMsg { ObjectType = documentObjectType, Prefix = prefix, RefId = refId });
        result = response.SeriaNumber;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<PagedResultDto<CommonUserDto>> GetAllUnitAndChildsMembersAsync(
        GetAllUnitAndChildsMembersRequestDto input)
    {

      try
      {
        var keyValue = await domainService.GetAllUnitAndChildsMembersAsync(input.OrgUnitId, input.Sorting, input.MaxResultCount, input.SkipCount, input.SearchQuery);

        var result = ObjectMapper.Map<List<IdentityUser>, List<CommonUserDto>>(keyValue.Value);

        if (result != null && result.Count > 0)
        {
          foreach (var user in result)
          {
            user.Roles = await roleDomainService.GetRolesByUserId(user.Id);
            user.OrganizationUnits = await GetUserOrganizationUnits(user.Id);
          }
        }

        return new PagedResultDto<CommonUserDto>(keyValue.Key, result);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }
    }
  }
}
