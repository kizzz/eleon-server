namespace Messaging.Module.Messages;

[Common.Module.Events.DistributedEvent]
public class LifecycleActorsForApprovalMsg : VportalEvent
{
  public required string DocumentId { get; set; }
  public required string DocumentObjectType { get; set; }
}
