using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Roles;
using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Users;
using Volo.Abp.Application.Dtos;

namespace Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.OrganizationUnits
{
  public interface IOrganizationUnitAppService
  {
    Task<List<UserOrganizationUnitLookupDto>> GetAvailableForUserAsync(GetAvatilableOrgUnitsInput input);
    Task<CommonOrganizationUnitDto> GetByIdAsync(Guid id, bool includeSoftDeleted = false);
    Task<List<CommonOrganizationUnitDto>> GetListAsync();
    Task<List<CommonUserDto>> GetMembers(CommonOrganizationUnitDto orgUnit);
    Task<List<CommonRoleDto>> GetRoles(CommonOrganizationUnitDto orgUnit);
    Task<List<CommonOrganizationUnitDto>> GetUserOrganizationUnits(Guid userId);
    Task<List<CommonOrganizationUnitDto>> GetRoleOrganizationUnits(string roleName);
    Task<List<CommonOrganizationUnitDto>> GetHighLevelOrgUnitListAsync();
    Task<bool> CheckOrganizationUnitPermissionAsync(Guid orgUnitId, string permission);
    Task<bool> CheckOrganizationUnitParentsPermissionAsync(Guid orgUnitId, string permission);
    Task<string> GetDocumentSeriaNumberAsync(string documentObjectType, string prefix, string refId);
    Task<PagedResultDto<CommonUserDto>> GetAllUnitAndChildsMembersAsync(GetAllUnitAndChildsMembersRequestDto input);
  }
}
