using Volo.Abp.Reflection;

namespace VPortal.TenantManagement.Module.Permissions
{
  public class VportalSpecialPermissions
  {
    public const string SuspendedAdmin = "SuspendedAdmin";
    public const string SuspendedUser = "SuspendedUser";

    public static string[] GetAll()
    {
      return ReflectionHelper.GetPublicConstantsRecursively(typeof(VportalSpecialPermissions));
    }

    public static bool IsSpecialPermission(string permission) => GetAll().Contains(permission);
  }
}
