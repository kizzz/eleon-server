namespace VPortal.JobScheduler.Module;

public static class JobSchedulerErrorCodes
{
  public const string CanOnlyRunReadyTask = "JobScheduler:010000";
  public const string TaskMustHaveActions = "JobScheduler:010001";
  public const string RunManuallyNotAllowed = "JobScheduler:010001";
  public const string FailedToRunTenantDueTasks = "JobScheduler:010010";
  public const string FailedToRunHostDueTasks = "JobScheduler:010011";
  public const string CyclicActionDependency = "JobScheduler:010012";
  public const string NowAllowedWhenTaskRunning = "JobScheduler:010013";
  public const string ActivateTaskNotAllowed = "JobScheduler:010014";
  public const string CanOnlyStopRunningTask = "JobScheduler:010015";
  public const string NonValidActionParameters = "JobScheduler:010016";
}
