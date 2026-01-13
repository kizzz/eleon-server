using Volo.Abp.Reflection;

namespace VPortal.Identity.Module.Permissions;

public class IdentityPermissions
{
  public const string GroupName = "Identity";

  public static class Session
  {
    public const string Default = GroupName + ".Sessions";

    public const string Revoke = Default + ".Revoke";
  }

  public static class ApiKey
  {
    public const string Default = GroupName + ".ApiKey";
  }

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(IdentityPermissions));
  }
}
