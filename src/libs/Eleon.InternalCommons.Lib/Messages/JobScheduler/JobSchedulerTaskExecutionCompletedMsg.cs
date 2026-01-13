using Common.Module.Constants;

namespace Messaging.Module.Messages;

[Common.Module.Events.DistributedEvent]
public class JobSchedulerTaskExecutionCompletedMsg : VportalEvent
{
  public JobSchedulerTaskExecutionStatus ExecutionResult { get; set; }
  public Guid ExecutionId { get; set; }
}
