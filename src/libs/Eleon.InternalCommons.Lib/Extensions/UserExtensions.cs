using Volo.Abp.Data;
using Volo.Abp.Identity;

namespace VPortal.TenantManagement.Module.Users
{
  public static class UserExtensions
  {
    public const string UserProfilePictureProp = "ProfilePicture";

    public const string UserProfilePictureThumbnailProp = "ProfilePictureThumbnail";

    public static string GetProfilePicture(this IdentityUser user)
    {
      return user.GetProperty<string>(UserProfilePictureProp);
    }

    public static void SetProfilePicture(this IdentityUser user, string profilePicture)
    {
      user.SetProperty(UserProfilePictureProp, profilePicture);
    }

    public static string GetProfilePictureThumbnail(this IdentityUser user)
    {
      return user.GetProperty<string>(UserProfilePictureThumbnailProp);
    }

    public static void SetProfilePictureThumbnail(this IdentityUser user, string profilePictureThumbnail)
    {
      user.SetProperty(UserProfilePictureThumbnailProp, profilePictureThumbnail);
    }
  }
}
