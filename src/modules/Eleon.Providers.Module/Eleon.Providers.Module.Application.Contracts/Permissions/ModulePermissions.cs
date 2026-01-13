using Volo.Abp.Reflection;

namespace VPortal.Storage.Module.Permissions;

public class ModulePermissions
{
  public const string GroupName = "StorageModule";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(ModulePermissions));
  }
}
