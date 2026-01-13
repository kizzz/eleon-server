using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Roles;
using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Users;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using VPortal.HealthCheckModule.Module;
using VPortal.TenantManagement.Module;

namespace VPortal.Core.Infrastructure.Module.Controllers
{
  [Area(IdentityQueryingRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = IdentityQueryingRemoteServiceConsts.RemoteServiceName)]
  [Route("api/identity-querying/users/")]
  public class UserQueryingController : IdentityQueryingModuleController, ICommonUserAppService
  {
    private readonly ICommonUserAppService commonUserApplicationService;

    public UserQueryingController(ICommonUserAppService commonUserApplicationService)
    {
      this.commonUserApplicationService = commonUserApplicationService;
    }

    [AllowAnonymous]
    [HttpPost("CheckPermission")]
    public async Task<bool> CheckPermission(string permission)
    {
      bool response = await commonUserApplicationService.CheckPermission(permission);


      return response;
    }

    [HttpGet("GetAllUsersListAsync")]
    public async Task<List<CommonUserDto>> GetAllUsersListAsync()
    {

      List<CommonUserDto> response = await commonUserApplicationService.GetAllUsersListAsync();


      return response;
    }

    [HttpGet("GetById")]
    public async Task<CommonUserDto> GetById(Guid id)
    {

      CommonUserDto response = await commonUserApplicationService.GetById(id);


      return response;
    }

    [HttpGet("GetCurrentUser")]
    public async Task<IdentityUserDto> GetCurrentUser()
    {

      var response = await commonUserApplicationService.GetCurrentUser();


      return response;
    }

    [HttpPost("GetList")]
    public async Task<PagedResultDto<CommonUserDto>> GetListAsync(GetCommonUsersInput input)
    {

      var response = await commonUserApplicationService.GetListAsync(input);


      return response;
    }

    [HttpGet("GetRoles")]
    public async Task<List<CommonRoleDto>> GetRoles(Guid id)
    {

      var response = await commonUserApplicationService.GetRoles(id);


      return response;
    }

    [HttpGet("FindByEmail")]
    public async Task<IdentityUserDto> FindByEmailAsync(string email)
    {

      var response = await commonUserApplicationService.FindByEmailAsync(email);


      return response;
    }

    [HttpGet("FindByUsername")]
    public async Task<IdentityUserDto> FindByUsernameAsync(string userName)
    {

      var response = await commonUserApplicationService.FindByUsernameAsync(userName);


      return response;
    }

    [HttpGet("GetIdentityUserRoles")]
    public async Task<ListResultDto<IdentityRoleDto>> GetIdentityRolesAsync(Guid id)
    {

      var response = await commonUserApplicationService.GetIdentityRolesAsync(id);


      return response;
    }

    [HttpGet("GetAssignableRoles")]
    public async Task<ListResultDto<IdentityRoleDto>> GetAssignableRolesAsync()
    {

      var response = await commonUserApplicationService.GetAssignableRolesAsync();


      return response;
    }

    [HttpGet("GetIdentityUser")]
    public async Task<IdentityUserDto> GetAsync(Guid id)
    {

      var response = await commonUserApplicationService.GetAsync(id);


      return response;
    }
  }
}
