using Volo.Abp.Reflection;

namespace VPortal.GatewayManagement.Module.Permissions;

public class GatewayManagementPermissions
{
  public const string GroupName = "GatewayManagement";

  public const string Gateway = GroupName + ".Gateway";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(GatewayManagementPermissions));
  }
}
