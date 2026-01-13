using Volo.Abp.Reflection;

namespace VPortal.Google.Module.Permissions;

public class GooglePermissions
{
  public const string GroupName = "Google";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(GooglePermissions));
  }
}
