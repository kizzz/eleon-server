using Common.Module.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using VPortal.TenantManagement.Module.Roles;

namespace VPortal.TenantManagement.Module.Repositories
{
  public class RoleLookupRepository : IRoleLookupRepository
  {
    private readonly IDbContextProvider<EleonAbpDbContext> dbContextProvider;

    public bool? IsChangeTrackingEnabled => false;
    public string EntityName { get; set; } = "Role";
    public string ProviderName { get; set; } = "Role";

    public RoleLookupRepository(IDbContextProvider<EleonAbpDbContext> dbContextProvider)
    {
      this.dbContextProvider = dbContextProvider;
    }

    public async Task<List<UserRoleLookup>> GetRolesByUserId(Guid userId, UserRoleLookupProviderFormat providerFormat)
    {
      var dbContext = await dbContextProvider.GetDbContextAsync();
      var userOwnRoleIds = await (from userRole in dbContext.Set<IdentityUserRole>()
                                  join role in dbContext.Roles on userRole.RoleId equals role.Id
                                  where userRole.UserId == userId
                                  select role.Id)
                  .ToListAsync();

      var userOrgUnitIds = (await dbContext.Set<IdentityUserOrganizationUnit>()
          .Where(q => q.UserId == userId)
          .Select(q => q.OrganizationUnitId)
          .ToListAsync())
          .ToArray();

      var userOrgUnitRoles = await (
          from ouRole in dbContext.Set<OrganizationUnitRole>()
          join ou in dbContext.Set<OrganizationUnit>() on ouRole.OrganizationUnitId equals ou.Id
          where userOrgUnitIds.Contains(ouRole.OrganizationUnitId)
          select new { OrgUnitName = ou.DisplayName, ouRole.RoleId, OrgUnitId = ou.Id, })
          .ToListAsync();

      var distinctRoleIds = userOrgUnitRoles
          .Select(x => x.RoleId)
          .Concat(userOwnRoleIds)
          .Distinct()
          .ToArray();

      var roles = await dbContext.Roles
          .Where(r => distinctRoleIds.Contains(r.Id))
          .Select(r => new { RoleId = r.Id, RoleName = r.Name })
          .ToListAsync();

      return roles
          .Select(x => new UserRoleLookup()
          {
            RoleName = x.RoleName,
            Providers = userOrgUnitRoles
                  .Where(y => y.RoleId == x.RoleId)
                  .Select(y => providerFormat switch
                  {
                    UserRoleLookupProviderFormat.Name => y.OrgUnitName,
                    UserRoleLookupProviderFormat.Id => y.OrgUnitId.ToString(),
                    _ => throw new NotImplementedException(),
                  })
                  .Distinct()
                  .ToList(),
          })
          .ToList();
    }

    public async Task<KeyValuePair<int, List<RoleUserLookup>>> GetUsersByRole(
        string roleName,
        string userNameFilter,
        int skip,
        int take,
        bool exclusionMode)
    {
      var dbContext = await dbContextProvider.GetDbContextAsync();

      var roleOrgUnits = await (
          from ouRole in dbContext.Set<OrganizationUnitRole>()
          join ou in dbContext.Set<OrganizationUnit>() on ouRole.OrganizationUnitId equals ou.Id
          join role in dbContext.Roles on ouRole.RoleId equals role.Id
          where role.Name == roleName
          select new { OrgUnitName = ou.DisplayName, OrgUnitId = ou.Id })
          .ToListAsync();

      var roleOrgUnitIds = roleOrgUnits
          .Select(x => x.OrgUnitId)
          .Distinct()
          .ToArray();

      var roleOrgUnitQuery = dbContext.Set<IdentityUserOrganizationUnit>()
          .Where(q => roleOrgUnitIds.Contains(q.OrganizationUnitId));

      var roleUserIdsQuery =
          from userRole in dbContext.Set<IdentityUserRole>()
          join role in dbContext.Roles on userRole.RoleId equals role.Id
          where role.Name == roleName
          select userRole.UserId;

      var orgUnitUserIdsQuery = roleOrgUnitQuery.Select(q => q.UserId);

      var allRoleUserIdsQuery = orgUnitUserIdsQuery.Union(roleUserIdsQuery);

      var allUsersQuery = exclusionMode
          ? (from user in dbContext.Users
             where !allRoleUserIdsQuery.Any(x => x == user.Id)
             select user)
          : (from user in dbContext.Users
             join userId in allRoleUserIdsQuery on user.Id equals userId
             select user);

      var pagedAndSortedUsers = await allUsersQuery
          .WhereIf(
              !string.IsNullOrEmpty(userNameFilter),
              user => user.UserName.Contains(userNameFilter) || (user.Name + " " + user.Surname).Contains(userNameFilter))
          .OrderBy(user => user.UserName)
          .Skip(skip)
          .Take(take)
          .ToListAsync();

      var userIdsOnPage = pagedAndSortedUsers.Select(x => x.Id).ToArray();
      var roleOrgUnitsOnPage = await roleOrgUnitQuery
          .Where(q => userIdsOnPage.Contains(q.UserId))
          .ToListAsync();
      var userOrgUnitsLookup = userIdsOnPage.ToDictionary(
          uid => uid,
          uid => roleOrgUnitsOnPage
              .Where(x => x.UserId == uid)
              .Select(x => roleOrgUnits.First(ou => ou.OrgUnitId == x.OrganizationUnitId))
              .Select(x => x.OrgUnitName)
              .Distinct()
              .ToList());

      var userRolesLookups = pagedAndSortedUsers.Select(u => new RoleUserLookup()
      {
        User = u,
        Providers = userOrgUnitsLookup.GetValueOrDefault(u.Id, []),
      }).ToList();

      var usersCount = await allUsersQuery.CountAsync();
      return KeyValuePair.Create(usersCount, userRolesLookups);
    }

    public async Task<List<IdentityRole>> GetRolesByUserIdAsync(Guid userId)
    {
      var dbContext = await dbContextProvider.GetDbContextAsync();
      var userOwnRoleIds = await (from userRole in dbContext.Set<IdentityUserRole>()
                                  join role in dbContext.Roles on userRole.RoleId equals role.Id
                                  where userRole.UserId == userId
                                  select role.Id)
                  .ToListAsync();

      var userOrgUnitIds = (await dbContext.Set<IdentityUserOrganizationUnit>()
          .Where(q => q.UserId == userId)
          .Select(q => q.OrganizationUnitId)
          .ToListAsync())
          .ToArray();

      var userOrgUnitRoles = await (
          from ouRole in dbContext.Set<OrganizationUnitRole>()
          join ou in dbContext.Set<OrganizationUnit>() on ouRole.OrganizationUnitId equals ou.Id
          where userOrgUnitIds.Contains(ouRole.OrganizationUnitId)
          select new { OrgUnitName = ou.DisplayName, ouRole.RoleId, OrgUnitId = ou.Id, })
          .ToListAsync();

      var distinctRoleIds = userOrgUnitRoles
          .Select(x => x.RoleId)
          .Concat(userOwnRoleIds)
          .Distinct()
          .ToArray();

      var roles = await dbContext.Roles
          .Where(r => distinctRoleIds.Contains(r.Id))
          .ToListAsync();

      return roles;
    }

    public async Task<IQueryable<IdentityRole>> GetQueryableAsync()
    {
      var dbContext = await dbContextProvider.GetDbContextAsync();
      return dbContext.Roles;
    }

    public async Task<List<string>> GetGrantedRolePermissions(Guid userId)//elina permission check
    {
      var dbContext = await dbContextProvider.GetDbContextAsync();

      var userRoles = (from userRole in dbContext.Set<IdentityUserRole>()
                       join role in dbContext.Set<IdentityRole>() on userRole.RoleId equals role.Id
                       where userRole.UserId == userId
                       select role.Name
                     ).ToList();


      var userOrgUnitRoles = (from userOrgUnit in dbContext.Set<IdentityUserOrganizationUnit>()
                              join orgUnitRole in dbContext.Set<OrganizationUnitRole>()
                              on userOrgUnit.OrganizationUnitId equals orgUnitRole.OrganizationUnitId into joinedOrgUnits
                              from orgRole in joinedOrgUnits.DefaultIfEmpty()
                              join role in dbContext.Set<IdentityRole>()
                              on orgRole.RoleId equals role.Id into joinedRoles
                              from role in joinedRoles.DefaultIfEmpty()
                              where userOrgUnit.UserId == userId
                              select role.Name).Distinct().ToList();

      var combinedRoles = userRoles.Union(userOrgUnitRoles).ToList();

      string providerName = RolePermissionValueProvider.ProviderName;
      var permissions = from permission in dbContext.PermissionGrants
                        where permission.ProviderName == providerName
                        where combinedRoles.Contains(permission.ProviderKey)
                        select permission.Name;

      return await permissions.ToListAsync();
    }
  }
}
