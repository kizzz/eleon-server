using Volo.Abp.Threading;

namespace VPortal.SitesManagement.Module.EntityFrameworkCore;

public static class SitesManagementEfCoreEntityExtensionMappings
{
  private static readonly OneTimeRunner OneTimeRunner = new();

  public static void Configure()
  {
    SitesManagementModuleExtensionConfigurator.Configure();

    OneTimeRunner.Run(() =>
    {
      //ObjectExtensionManager.Instance
      //    .MapEfCoreProperty<IdentityUser, string>(UserExtensions.UserProfilePictureProp)
      //    .MapEfCoreProperty<IdentityUser, string>(UserExtensions.UserProfilePictureThumbnailProp);
    });
  }
}


