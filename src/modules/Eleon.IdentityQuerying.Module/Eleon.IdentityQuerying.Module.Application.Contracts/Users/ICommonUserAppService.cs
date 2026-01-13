using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Roles;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Users
{
  public interface ICommonUserAppService : IApplicationService
  {
    Task<PagedResultDto<CommonUserDto>> GetListAsync(GetCommonUsersInput input);
    Task<List<CommonUserDto>> GetAllUsersListAsync();
    Task<CommonUserDto> GetById(Guid id);//elina
    Task<List<CommonRoleDto>> GetRoles(Guid id);
    Task<bool> CheckPermission(string permission);
    Task<IdentityUserDto> GetCurrentUser();

    Task<ListResultDto<IdentityRoleDto>> GetIdentityRolesAsync(Guid id);
    Task<ListResultDto<IdentityRoleDto>> GetAssignableRolesAsync();
    Task<IdentityUserDto> FindByUsernameAsync(string userName);
    Task<IdentityUserDto> FindByEmailAsync(string email);
    Task<IdentityUserDto> GetAsync(Guid id);
  }
}
