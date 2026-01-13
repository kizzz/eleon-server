using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using VPortal.TenantManagement.Module.Roles;

namespace VPortal.TenantManagement.Module.Repositories
{
  public interface IRoleLookupRepository : IRepository
  {
    Task<List<IdentityRole>> GetRolesByUserIdAsync(Guid userId);
    Task<List<UserRoleLookup>> GetRolesByUserId(Guid userId, UserRoleLookupProviderFormat providerFormat);
    Task<KeyValuePair<int, List<RoleUserLookup>>> GetUsersByRole(
        string roleName,
        string userNameFilter,
        int skip,
        int take,
        bool exclusionMode);
    Task<IQueryable<IdentityRole>> GetQueryableAsync();

    Task<List<string>> GetGrantedRolePermissions(Guid userId);
  }
}
