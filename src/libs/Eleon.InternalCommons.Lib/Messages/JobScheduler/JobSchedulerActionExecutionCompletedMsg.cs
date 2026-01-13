using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class JobSchedulerActionExecutionCompletedMsg : VportalEvent
  {
    public JobSchedulerExecutionResult ExecutionResult { get; set; }
    public string ActionName { get; set; }
    public string EventName { get; set; }
  }
}
