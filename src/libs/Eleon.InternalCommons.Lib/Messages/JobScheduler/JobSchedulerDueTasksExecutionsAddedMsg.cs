using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class JobSchedulerDueTasksExecutionsAddedMsg : VportalEvent
  {
    public bool Success { get; set; }
    public List<Guid?> FailedTenantsIds { get; set; }
    public List<string> TenantTasksExecutionsAddErrorCodes { get; set; }
  }
}
