using Volo.Abp.Reflection;

namespace VPortal.Auditor.Module.Permissions;

public class ModulePermissions
{
  public const string GroupName = "Auditor.Module";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(ModulePermissions));
  }
}
