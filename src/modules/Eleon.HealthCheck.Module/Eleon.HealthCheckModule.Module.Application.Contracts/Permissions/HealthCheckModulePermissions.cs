using Volo.Abp.Reflection;

namespace VPortal.HealthCheckModule.Permissions;

public class HealthCheckModulePermissions
{
  public const string GroupName = "HealthCheck";

  public const string Default = GroupName + ".Default";
  public const string HealthCheckManager = GroupName + ".HealthCheckManager";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(HealthCheckModulePermissions));
  }
}
