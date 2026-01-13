using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class CancelLifecycleMsg : VportalEvent
  {
    public string DocumentId { get; set; }

    public string DocumentObjectType { get; set; }
  }
}
