using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;

namespace VPortal.TenantManagement.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/TenantManagement/ProfileSettingsController")]
  public class ProfileController : TenantManagementController, IProfileAppService
  {
    private readonly IVportalLogger<ProfileController> logger;
    private readonly IProfileAppService profileAppService;

    public ProfileController(
        IVportalLogger<ProfileController> logger,
        IProfileAppService profileAppService)
    {
      this.logger = logger;
      this.profileAppService = profileAppService;
    }

    [HttpPost("ChangePasswordAsync")]
    public async Task ChangePasswordAsync(ChangePasswordInput input)
    {

      await profileAppService.ChangePasswordAsync(input);

    }

    [HttpPost("UpdateAsync")]
    public async Task<ProfileDto> UpdateAsync(UpdateProfileDto input)
    {

      var response = await profileAppService.UpdateAsync(input);

      return response;
    }

    [HttpGet("GetAsync")]
    public async Task<ProfileDto> GetAsync()
    {

      var response = await profileAppService.GetAsync();

      return response;
    }
  }
}
