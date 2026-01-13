using Common.Module.Constants;
using Volo.Abp.Reflection;

namespace VPortal.JobScheduler.Module.Permissions;

public class JobSchedulerPermissions
{
  public const string GroupName = $"JobSchedule";
  public const string General = $"Permission.JobSchedule.General";
  public const string Create = $"Permission.JobSchedule.Create";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(JobSchedulerPermissions));
  }
}
