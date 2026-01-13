using Volo.Abp.Reflection;

namespace VPortal.ApplicationConfiguration.Module.Permissions;

public class ApplicationConfigurationPermissions
{
  public const string GroupName = "ApplicationConfiguration";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(ApplicationConfigurationPermissions));
  }
}
