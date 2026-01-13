using Volo.Abp.Reflection;

namespace VPortal.EventManagementModule.Permissions;

public class EventManagementModulePermissions
{
  public const string GroupName = "EventManagementModule";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(EventManagementModulePermissions));
  }
}
