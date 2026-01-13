using Common.EventBus.Module;
using Eleon.InternalCommons.Lib.Messages.UserRole;
using Logging.Module;
using Migrations.Module;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.TenantManagement.Module.Roles;

namespace VPortal.TenantManagement.Module.DomainServices
{

  public class RoleDomainService : DomainService
  {
    private readonly ICurrentTenant currentTenant;
    private readonly IVportalLogger<RoleDomainService> logger;
    private readonly IIdentityRoleRepository identityRoleRepository;
    private readonly IdentityUserManager userManager;
    private readonly ICurrentUser currentUser;
    private readonly IDistributedEventBus _distributedEventBus;

    public RoleDomainService(
            ICurrentTenant currentTenant,
            IVportalLogger<RoleDomainService> logger,
            IIdentityRoleRepository identityRoleRepository,
            IdentityUserManager userManager,
            ICurrentUser currentUser,
            IDistributedEventBus distributedEventBus)
    {
      this.currentTenant = currentTenant;
      this.logger = logger;
      this.identityRoleRepository = identityRoleRepository;
      this.userManager = userManager;
      this.currentUser = currentUser;
      _distributedEventBus = distributedEventBus;
    }

    public async Task<KeyValuePair<long, List<IdentityRole>>> GetListAsync(
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string filter = null)
    {
      KeyValuePair<long, List<IdentityRole>> result = new KeyValuePair<long, List<IdentityRole>>();
      try
      {
        result = new KeyValuePair<long, List<IdentityRole>>(
            await identityRoleRepository.GetCountAsync(filter),
            await identityRoleRepository.GetListAsync(sorting, maxResultCount, skipCount, filter));
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

    public async Task<IList<string>> GetRolesByUserId(Guid id)
    {
      IList<string> result = null;
      try
      {
        var user = await userManager.GetByIdAsync(id);
        result = await userManager.GetRolesAsync(user);
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

    public async Task<IList<UserRoleLookup>> GetUserRolesLookup(Guid userId)
    {
      IList<UserRoleLookup> result = null;
      try
      {
        var response = await _distributedEventBus.RequestAsync<GetRolesByUserIdResponseMsg>(new GetRolesByUserIdMsg
        {
          UserId = userId,
          ProviderFormat = UserRoleLookupProviderFormat.Name
        });
        result = response.Roles;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<KeyValuePair<int, List<RoleUserLookup>>> GetUsersInRole(
        string roleName,
        string userNameFilter,
        int skip,
        int take,
        bool exclusionMode)
    {
      KeyValuePair<int, List<RoleUserLookup>> result = default;
      try
      {
        var response = await _distributedEventBus.RequestAsync<GetUsersInRoleResponseMsg>(new GetUsersInRoleMsg
        {
          RoleName = roleName,
          UserNameFilter = userNameFilter,
          Skip = skip,
          Take = take,
          ExclusionMode = exclusionMode
        });
        result = KeyValuePair.Create(response.TotalCount, response.Users);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task RemoveUserFromRole(Guid userId, string roleName)
    {
      try
      {
        var user = await userManager.GetByIdAsync(userId);
        await userManager.RemoveFromRoleAsync(user, roleName);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task AddUserToRole(Guid userId, string roleName)
    {
      try
      {
        var user = await userManager.GetByIdAsync(userId);
        await userManager.AddToRoleAsync(user, roleName);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<bool> IsUserAdmin()
    {
      bool result = false;
      try
      {
        var user = await userManager.GetByIdAsync(currentUser.Id.Value);
        result = await userManager.IsInRoleAsync(user, MigrationConsts.AdminRoleNameDefaultValue);
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
    public async Task<IdentityRole> CreateRoleAsync(string roleName, bool isDefault, bool isPublic)
    {
      IdentityRole result = null;
      try
      {
        var role = new IdentityRole(Guid.NewGuid(), roleName)
        {
          IsDefault = isDefault,
          IsPublic = isPublic,
        };
        result = await identityRoleRepository.InsertAsync(role, autoSave: true);
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

    public async Task<IdentityRole> UpdateRoleAsync(Guid roleId, string newName, bool isDefault, bool isPublic)
    {
      IdentityRole result = null;
      try
      {
        var role = await identityRoleRepository.GetAsync(roleId);
        role.ChangeName(newName);
        role.IsDefault = isDefault;
        role.IsPublic = isPublic;
        result = await identityRoleRepository.UpdateAsync(role, autoSave: true);
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

    public async Task DeleteRoleAsync(Guid roleId)
    {
      try
      {
        await identityRoleRepository.DeleteAsync(roleId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
    }

    public async Task<IdentityRole> GetRoleByIdAsync(Guid roleId)
    {
      IdentityRole result = null;
      try
      {
        result = await identityRoleRepository.GetAsync(roleId);
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
  }
}
