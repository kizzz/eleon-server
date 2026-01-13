using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;

namespace VPortal.TenantManagement.Module.Repositories
{
  [ExposeServices(typeof(ICommonUsersRepository))]
  public class CommonUsersRepository : ICommonUsersRepository, IScopedDependency
  {
    private readonly IPermissionManager permissionManager;
    private readonly IVportalLogger<CommonUsersRepository> logger;
    private readonly IIdentityUserRepository _userRepository;

    public CommonUsersRepository(
        IPermissionManager permissionManager,
        IVportalLogger<CommonUsersRepository> logger,
        IIdentityUserRepository userRepository)
    {
      this.logger = logger;
      _userRepository = userRepository;
      this.permissionManager = permissionManager;
    }

    public async Task<List<IdentityUser>> GetByIds(List<Guid> ids)
    {
      List<IdentityUser> result = null;
      try
      {
        var queryable = await _userRepository.GetDbSetAsync();
        result = await queryable
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }

      return result;
    }

    public async Task<int> GetCountAsync(string filter = null, string permissions = null, List<Guid> ignoredUsers = null)
    {
      int result = 0;
      try
      {
        var queryable = await _userRepository.GetDbSetAsync();
        var users = queryable
            .WhereIf(filter != null, u => u.Name.Contains(filter) || u.UserName.Contains(filter) || u.Email.Contains(filter) || u.PhoneNumber.Contains(filter))
            .WhereIf(ignoredUsers != null && ignoredUsers.Count > 0, u => !ignoredUsers.Contains(u.Id));

        if (permissions == null)
        {
          return await users.CountAsync();
        }
        foreach (var user in users)
        {
          var userPermissions = await permissionManager.GetAllForUserAsync(user.Id);
          if (userPermissions
              .Where(p => p.IsGranted)
              .Select(p => p.Name)
              .Intersect(permissions.Split(","))
              .Count() == permissions.Split(",").Count())
          {
            result++;
          }
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }

      return result;
    }

    public async Task<List<IdentityUser>> GetListAsync(string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, string filter = null, string permissions = null, List<Guid> ignoredUsers = null)
    {
      var result = new List<IdentityUser>();
      try
      {
        var queryable = await _userRepository.GetDbSetAsync();
        var users = queryable
            .WhereIf(filter != null, u => u.Name.Contains(filter) || u.UserName.Contains(filter) || u.Email.Contains(filter) || u.PhoneNumber.Contains(filter))
            .WhereIf(ignoredUsers != null && ignoredUsers.Count > 0, u => !ignoredUsers.Contains(u.Id));


        if (!string.IsNullOrEmpty(sorting))
        {
          users = users.OrderBy(sorting);
        }

        if (string.IsNullOrEmpty(permissions))
        {
          return users
          .Skip(skipCount)
          .Take(maxResultCount)
          .ToList();
        }

        foreach (var user in users)
        {
          var userPermissions = await permissionManager.GetAllForUserAsync(user.Id);
          if (userPermissions
              .Where(p => p.IsGranted)
              .Select(p => p.Name)
              .Intersect(permissions.Split(","))
              .Count() == permissions.Split(",").Count())
          {
            result.Add(user);
          }
        }

        result = result
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToList();
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }

      return result;
    }
  }

}
