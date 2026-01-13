using Common.Module.ValueObjects;
using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Roles;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using VPortal.HealthCheckModule.Module;
using VPortal.TenantManagement.Module.DomainServices;
using VPortal.TenantManagement.Module.Roles;

namespace VPortal.Core.Infrastructure.Module.Roles
{
  public class RoleAppService : IdentityQueryingAppService, IRoleAppService
  {
    private readonly RoleDomainService roleDomainService;
    private readonly IVportalLogger<RoleAppService> logger;
    private readonly IIdentityRoleAppService _abpAppService;

    public RoleAppService(
        RoleDomainService roleDomainService,
        IVportalLogger<RoleAppService> logger,
        IdentityRoleManager roleManager,
        IIdentityRoleRepository roleRepository,
        IIdentityRoleAppService abpAppService)
    { 
      this.roleDomainService = roleDomainService;
      this.logger = logger;
      _abpAppService = abpAppService;
    }

    [Authorize("AbpIdentity.Roles")]
    public async Task<PagedResultDto<CommonRoleDto>> GetListAsync(GetCommonRolesInput input)
    {
      PagedResultDto<CommonRoleDto> response = null;
      try
      {
        KeyValuePair<long, List<IdentityRole>> source = await roleDomainService.GetListAsync(
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount,
            input.Filter);

        var roles = source.Value.Select(r => new CommonRoleValueObject()
        {
          Id = r.Id,
          Name = r.Name,
          InheritedFrom = null,
          IsReadOnly = false,
          IsPublic = r.IsPublic,
          IsDefault = r.IsDefault,
        }).ToList();

        var items = ObjectMapper.Map<List<CommonRoleValueObject>, List<CommonRoleDto>>(roles);

        response = new PagedResultDto<CommonRoleDto>()
        {
          Items = items,
          TotalCount = source.Key,
        };
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<IList<UserRoleLookupDto>> GetUserRolesLookup(Guid userId)
    {
      IList<UserRoleLookupDto> response = null;
      try
      {
        var users = await roleDomainService.GetUserRolesLookup(userId);
        response = ObjectMapper.Map<List<UserRoleLookup>, List<UserRoleLookupDto>>([.. users]);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<PagedResultDto<RoleUserLookupDto>> GetUsersInRole(GetUsersInRoleInput input)
    {
      PagedResultDto<RoleUserLookupDto> response = null;
      try
      {
        var users = await roleDomainService.GetUsersInRole(
            input.RoleName,
            input.UserNameFilter,
            input.SkipCount,
            input.MaxResultCount,
            input.ExclusionMode);

        var dtos = ObjectMapper.Map<List<RoleUserLookup>, List<RoleUserLookupDto>>(users.Value);
        response = new(users.Key, dtos);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<bool> IsUserAdmin()
    {
      bool result = false;
      try
      {
        result = await roleDomainService.IsUserAdmin();
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

    public Task<ListResultDto<IdentityRoleDto>> GetAllListAsync()
    {
      return _abpAppService.GetAllListAsync();
    }

    public Task<IdentityRoleDto> GetAsync(Guid id)
    {
      return _abpAppService.GetAsync(id);
    }
  }
}
