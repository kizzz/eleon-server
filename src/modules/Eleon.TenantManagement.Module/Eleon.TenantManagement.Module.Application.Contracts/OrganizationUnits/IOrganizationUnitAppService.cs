using Volo.Abp.Application.Dtos;
using VPortal.TenantManagement.Module.Roles;
using VPortal.TenantManagement.Module.Users;

namespace VPortal.TenantManagement.Module.OrganizationUnits
{
  public interface IOrganizationUnitAppService
  {
    Task<CommonOrganizationUnitTreeNodeDto> Clone(Guid id, string newName, bool withRoles, bool withMembers, bool withChildren);
    Task<bool> Move(Guid id, Guid newParentId);
    Task<List<UserOrganizationUnitLookupDto>> GetAvailableForUserAsync(GetAvatilableOrgUnitsInput input);
    Task<CommonOrganizationUnitDto> GetByIdAsync(Guid id, bool includeSoftDeleted = false);
    Task<List<CommonOrganizationUnitDto>> GetListAsync();
    Task<CommonOrganizationUnitDto> CreateAsync(CommonOrganizationUnitDto orgUnit);
    Task<CommonOrganizationUnitDto> UpdateAsync(CommonOrganizationUnitDto orgUnit);
    Task DeleteAsync(Guid orgUnitId);
    Task AddMemberAsync(Guid userId, Guid orgUnitId);
    Task AddMembersAsync(Guid orgUnitId, List<Guid> userIds);
    Task RemoveMember(Guid userId, Guid orgUnitId);
    Task AddRoleAsync(Guid roleId, Guid orgUnitId);
    Task AddRolesAsync(Guid orgUnitId, List<Guid> roleIds);
    Task RemoveRole(Guid roleId, Guid orgUnitId);
    Task<List<CommonUserDto>> GetMembers(CommonOrganizationUnitDto orgUnit);
    Task<List<CommonRoleDto>> GetRoles(CommonOrganizationUnitDto orgUnit);
    Task<List<CommonOrganizationUnitDto>> GetUserOrganizationUnits(Guid userId);
    Task<bool> SetUserOrganizationUnits(SetUserOrganizationUnitsInput input);
    Task<List<CommonOrganizationUnitDto>> GetRoleOrganizationUnits(string roleName);
    Task<bool> SetRoleOrganizationUnits(SetRoleOrganizationUnitsInput input);
    Task<List<CommonOrganizationUnitDto>> GetHighLevelOrgUnitListAsync();
    Task<bool> CheckOrganizationUnitPermissionAsync(Guid orgUnitId, string permission);
    Task<bool> CheckOrganizationUnitParentsPermissionAsync(Guid orgUnitId, string permission);
    Task<string> GetDocumentSeriaNumberAsync(string documentObjectType, string prefix, string refId);
    Task<PagedResultDto<CommonUserDto>> GetAllUnitAndChildsMembersAsync(GetAllUnitAndChildsMembersRequestDto input);
  }
}
