using Volo.Abp.Threading;

namespace VPortal.TenantManagement.Module.EntityFrameworkCore;

public static class TenantManagementEfCoreEntityExtensionMappings
{
  private static readonly OneTimeRunner OneTimeRunner = new();

  public static void Configure()
  {
    TenantManagementModuleExtensionConfigurator.Configure();

    OneTimeRunner.Run(() =>
    {
      //ObjectExtensionManager.Instance
      //    .MapEfCoreProperty<IdentityUser, string>(UserExtensions.UserProfilePictureProp)
      //    .MapEfCoreProperty<IdentityUser, string>(UserExtensions.UserProfilePictureThumbnailProp);
    });
  }
}
