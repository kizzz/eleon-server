namespace ModuleCollector.JobScheduler.Module.JobScheduler.Module.Domain.Shared.Constants
{
  [Flags]
  public enum DaysOfWeekOccurencesMasks
  {
    None = 0,
    First = 1 << 0,
    Second = 1 << 1,
    Third = 1 << 2,
    Fourth = 1 << 3,
    Last = 1 << 4,
  }
}
