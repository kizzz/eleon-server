using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using VPortal.TenantManagement.Module.DomainServices;

namespace VPortal.TenantManagement.Module.Users
{
  [Authorize]
  public class UserProfilePictureAppService : TenantManagementAppService, IUserProfilePictureAppService
  {
    private readonly IVportalLogger<UserProfilePictureAppService> logger;
    private readonly UserProfilePictureDomainService domainService;

    public UserProfilePictureAppService(
        IVportalLogger<UserProfilePictureAppService> logger,
        UserProfilePictureDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<bool> SetUserProfilePicture(SetUserProfilePictureRequest request)
    {
      bool result = false;
      try
      {
        await domainService.SetUserProfilePictures(
            CurrentUser.Id.Value,
            request.ProfilePictureBase64,
            request.ProfilePictureThumbnailBase64);

        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
