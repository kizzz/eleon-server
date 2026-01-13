using Volo.Abp.Reflection;

namespace VPortal.Notificator.Module.Permissions;

public class NotificatorPermissions
{
  public const string GroupName = "Notificator";

  public const string General = "Notificator.General";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(NotificatorPermissions));
  }
}
