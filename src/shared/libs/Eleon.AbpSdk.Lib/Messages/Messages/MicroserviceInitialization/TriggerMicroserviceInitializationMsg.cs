namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class TriggerMicroserviceInitializationMsg : VportalEvent
  {
    public Guid RequestId { get; set; }
  }
}
