using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.TenantManagement.Module.UserOtpSettings;

namespace VPortal.TenantManagement.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Infrastructure/UserOtpSettings/")]
  public class UserOtpSettingsController : TenantManagementController, IUserOtpSettingsAppService
  {
    private readonly IVportalLogger<UserOtpSettingsController> logger;
    private readonly IUserOtpSettingsAppService appService;

    public UserOtpSettingsController(
        IVportalLogger<UserOtpSettingsController> logger,
        IUserOtpSettingsAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }

    [HttpGet("GetUserOtpSettings")]
    public async Task<UserOtpSettingsDto> GetUserOtpSettings(Guid userId)
    {
      var result = await appService.GetUserOtpSettings(userId);
      return result;
    }

    [HttpPost("SetUserOtpSettings")]
    public async Task<bool> SetUserOtpSettings(UserOtpSettingsDto userOtpSettings)
    {
      var result = await appService.SetUserOtpSettings(userOtpSettings);
      return result;
    }
  }
}
