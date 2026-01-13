using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using VPortal.TenantManagement.Module.Roles;

namespace VPortal.TenantManagement.Module.Users
{
  public interface ICommonUserAppService : IApplicationService
  {
    Task<PagedResultDto<CommonUserDto>> GetListAsync(GetCommonUsersInput input);
    Task<List<CommonUserDto>> GetAllUsersListAsync();
    Task<ImportExcelUsersValueObjectDto> ImportExcelUsers(string file);
    Task<CommonUserDto> GetById(Guid id);//elina
    Task<List<CommonRoleDto>> GetRoles(Guid id);
    Task<bool> SetNewPassword(Guid userId, string newPassword);
    Task<bool> CheckPermission(string permission);
    Task<IdentityUserDto> GetCurrentUser();

    Task<ListResultDto<IdentityRoleDto>> GetIdentityRolesAsync(Guid id);
    Task<ListResultDto<IdentityRoleDto>> GetAssignableRolesAsync();
    Task UpdateRolesAsync(Guid id, IdentityUserUpdateRolesDto input);
    Task<IdentityUserDto> FindByUsernameAsync(string userName);
    Task<IdentityUserDto> FindByEmailAsync(string email);

    Task<IdentityUserDto> CreateAsync(IdentityUserCreateDto input);
    Task DeleteAsync(Guid id);
    Task<IdentityUserDto> UpdateAsync(Guid id, IdentityUserUpdateDto input);
    Task<IdentityUserDto> GetAsync(Guid id);

    Task<bool> IgnoreLastUserOtpAsync(Guid userId);
  }
}
