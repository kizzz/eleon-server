using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using VPortal.TenantManagement.Module;
using VPortal.TenantManagement.Module.Roles;
using VPortal.TenantManagement.Module.Users;

namespace VPortal.Core.Infrastructure.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Infrastructure/Users/")]
  public class CommonUserController : TenantManagementController, ICommonUserAppService
  {
    private readonly ICommonUserAppService commonUserApplicationService;
    private readonly IVportalLogger<CommonUserController> logger;

    public CommonUserController(
        ICommonUserAppService commonUserApplicationService,
        IVportalLogger<CommonUserController> logger)
    {
      this.commonUserApplicationService = commonUserApplicationService;
      this.logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("CheckPermission")]
    public async Task<bool> CheckPermission(string permission)
    {

      bool response = await commonUserApplicationService.CheckPermission(permission);


      return response;
    }

    [HttpGet]
    [Route("GetAllUsersListAsync")]
    public async Task<List<CommonUserDto>> GetAllUsersListAsync()
    {

      List<CommonUserDto> response = await commonUserApplicationService.GetAllUsersListAsync();


      return response;
    }

    [HttpGet]
    [Route("GetById")]
    public async Task<CommonUserDto> GetById(Guid id)
    {

      CommonUserDto response = await commonUserApplicationService.GetById(id);


      return response;
    }

    [HttpGet]
    [Route("GetCurrentUser")]
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

    [HttpGet]
    [Route("GetRoles")]
    public async Task<List<CommonRoleDto>> GetRoles(Guid id)
    {

      var response = await commonUserApplicationService.GetRoles(id);


      return response;
    }

    [HttpPost]
    [Route("ImportExcelUsers")]
    public async Task<ImportExcelUsersValueObjectDto> ImportExcelUsers(string file)
    {

      var response = await commonUserApplicationService.ImportExcelUsers(file);


      return response;
    }

    [HttpPost]
    [Route("SetNewPassword")]
    public async Task<bool> SetNewPassword(Guid userId, string newPassword)
    {

      var response = await commonUserApplicationService.SetNewPassword(userId, newPassword);


      return response;
    }

    [HttpPost]
    [Route("CreateIdentityUser")]
    public async Task<IdentityUserDto> CreateAsync(IdentityUserCreateDto input)
    {

      var response = await commonUserApplicationService.CreateAsync(input);


      return response;
    }

    [HttpDelete]
    [Route("DeleteIdentityUser")]
    public async Task DeleteAsync(Guid id)
    {

      await commonUserApplicationService.DeleteAsync(id);

    }

    [HttpGet]
    [Route("FindByEmail")]
    public async Task<IdentityUserDto> FindByEmailAsync(string email)
    {

      var response = await commonUserApplicationService.FindByEmailAsync(email);


      return response;
    }

    [HttpGet]
    [Route("FindByUsername")]
    public async Task<IdentityUserDto> FindByUsernameAsync(string userName)
    {

      var response = await commonUserApplicationService.FindByUsernameAsync(userName);


      return response;
    }

    [HttpPut]
    [Route("UpdateIdentityUser")]
    public async Task<IdentityUserDto> UpdateAsync(Guid id, IdentityUserUpdateDto input)
    {

      var response = await commonUserApplicationService.UpdateAsync(id, input);


      return response;
    }

    [HttpPut]
    [Route("UpdateIdentityUserRoles")]
    public async Task UpdateRolesAsync(Guid id, IdentityUserUpdateRolesDto input)
    {

      await commonUserApplicationService.UpdateRolesAsync(id, input);

    }

    [HttpGet]
    [Route("GetIdentityUserRoles")]
    public async Task<ListResultDto<IdentityRoleDto>> GetIdentityRolesAsync(Guid id)
    {

      var response = await commonUserApplicationService.GetIdentityRolesAsync(id);


      return response;
    }

    [HttpGet]
    [Route("GetAssignableRoles")]
    public async Task<ListResultDto<IdentityRoleDto>> GetAssignableRolesAsync()
    {

      var response = await commonUserApplicationService.GetAssignableRolesAsync();


      return response;
    }

    [HttpGet]
    [Route("GetIdentityUser")]
    public async Task<IdentityUserDto> GetAsync(Guid id)
    {

      var response = await commonUserApplicationService.GetAsync(id);


      return response;
    }

    [HttpPost("IgnoreLastUserOtp")]
    public async Task<bool> IgnoreLastUserOtpAsync(Guid userId)
    {

      var response = await commonUserApplicationService.IgnoreLastUserOtpAsync(userId);


      return response;
    }
  }
}
