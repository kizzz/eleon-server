using Volo.Abp.Reflection;

namespace VPortal.Lifecycle.Feature.Module.Permissions;

public class LifecyclePermissions
{
  public const string GroupName = "LifecycleFeatureModule";
  public const string General = $"{GroupName}.General";
  public const string LifecycleManager = $"{GroupName}.LifecycleManager";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(LifecyclePermissions));
  }
}
