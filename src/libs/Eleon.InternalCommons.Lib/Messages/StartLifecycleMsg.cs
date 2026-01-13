using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class StartLifecycleMsg : VportalEvent
  {
    public string DocumentId { get; set; }

    public string DocEntry { get; set; }
    public string DocumentObjectType { get; set; }
  }
}
