using Volo.Abp.Reflection;

namespace VPortal.BackgroundJobs.Module.Permissions;

public class ModulePermissions
{
  public const string GroupName = "BackgroundJob";
  public const string General = "Permission.BackgroundJob.General";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(ModulePermissions));
  }
}
