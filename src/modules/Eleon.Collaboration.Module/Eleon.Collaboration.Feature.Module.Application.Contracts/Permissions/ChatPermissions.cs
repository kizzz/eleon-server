using Volo.Abp.Reflection;

namespace VPortal.Collaboration.Feature.Module.Permissions;

public class ChatPermissions
{
  public const string GroupName = "Chat";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(ChatPermissions));
  }
}
