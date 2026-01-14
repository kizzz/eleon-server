using Volo.Abp.Reflection;

namespace VPortal.SystemServicesModule.Permissions;

public class SystemServicesModulePermissions
{
  public const string GroupName = "SystemServices";

  public const string Default = GroupName + ".Default";
  public const string SystemServicesManager = GroupName + ".SystemServicesManager";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(SystemServicesModulePermissions));
  }
}

