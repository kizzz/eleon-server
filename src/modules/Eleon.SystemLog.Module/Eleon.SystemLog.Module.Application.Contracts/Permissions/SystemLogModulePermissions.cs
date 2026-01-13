using Volo.Abp.Reflection;

namespace VPortal.DocMessageLog.Module.Permissions;

public class SystemLogModulePermissions
{
  public const string GroupName = "SystemLog";
  public const string General = "Permission.SystemLog.General";
  public const string ViewSecurityLogs = "Permission.SystemLog.ViewSecurityLogs";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(SystemLogModulePermissions));
  }
}
