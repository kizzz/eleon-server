using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class JobSchedulerActionExecutionInitializedMsg : VportalEvent
  {
    public Guid ActionExecutionId { get; set; }
  }
}
