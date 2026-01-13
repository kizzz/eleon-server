using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace VPortal.TenantManagement.Module.Roles
{
  public interface IRoleAppService : IApplicationService
  {
    Task<PagedResultDto<CommonRoleDto>> GetListAsync(GetCommonRolesInput input);
    Task<PagedResultDto<RoleUserLookupDto>> GetUsersInRole(GetUsersInRoleInput input);
    Task<bool> RemoveUserFromRole(RemoveUserFromRoleInput input);
    Task<bool> AddUserToRole(AddUserToRoleInput input);
    Task<IList<UserRoleLookupDto>> GetUserRolesLookup(Guid userId);
    Task<bool> IsUserAdmin();

    Task<IdentityRoleDto> CreateAsync(IdentityRoleCreateDto input);
    Task DeleteAsync(Guid id);
    Task<ListResultDto<IdentityRoleDto>> GetAllListAsync();
    Task<IdentityRoleDto> GetAsync(Guid id);
    Task<IdentityRoleDto> UpdateAsync(Guid id, IdentityRoleUpdateDto input);
  }
}
