using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.TenantManagement.Module.Users;

namespace VPortal.TenantManagement.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/CoreInfrastructure/UserProfilePicture")]
  public class UserProfilePictureController : TenantManagementController, IUserProfilePictureAppService
  {
    private readonly IUserProfilePictureAppService appService;
    private readonly IVportalLogger<UserProfilePictureController> logger;

    public UserProfilePictureController(
        IUserProfilePictureAppService appService,
        IVportalLogger<UserProfilePictureController> logger)
    {
      this.appService = appService;
      this.logger = logger;
    }

    [HttpPost("SetUserProfilePicture")]
    public async Task<bool> SetUserProfilePicture(SetUserProfilePictureRequest request)
    {

      var response = await appService.SetUserProfilePicture(request);


      return response;
    }
  }
}
