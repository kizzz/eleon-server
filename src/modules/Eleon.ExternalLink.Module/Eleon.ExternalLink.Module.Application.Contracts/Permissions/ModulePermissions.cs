using Volo.Abp.Reflection;

namespace VPortal.ExternalLink.Module.Permissions;

public class ModulePermissions
{
  public const string GroupName = "ExternalLink";
  public const string General = "Permission.ExternalLink.General";
  public const string Create = "Permission.ExternalLink.Create";
  public const string Share = "Permission.ExternalLink.Share";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(ModulePermissions));
  }
}
