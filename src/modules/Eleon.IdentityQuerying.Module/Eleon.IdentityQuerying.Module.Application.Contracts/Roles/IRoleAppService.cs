using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Roles
{
  public interface IRoleAppService : IApplicationService
  {
    Task<PagedResultDto<CommonRoleDto>> GetListAsync(GetCommonRolesInput input);
    Task<PagedResultDto<RoleUserLookupDto>> GetUsersInRole(GetUsersInRoleInput input);
    Task<IList<UserRoleLookupDto>> GetUserRolesLookup(Guid userId);
    Task<bool> IsUserAdmin();

    Task<ListResultDto<IdentityRoleDto>> GetAllListAsync();
    Task<IdentityRoleDto> GetAsync(Guid id);
  }
}
