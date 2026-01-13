using Volo.Abp.Reflection;

namespace VPortal.Infrastructure.Module.Permissions;

public class InfarastructurePermissions
{
  public const string GroupName = "InfrastructureModule";

  public const string ViewSecurityLogs = $"Permission.{GroupName}.ViewSecurityLogs";

  public const string SendNotification = $"Permission.{GroupName}.SendNotification";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(InfarastructurePermissions));
  }
}
