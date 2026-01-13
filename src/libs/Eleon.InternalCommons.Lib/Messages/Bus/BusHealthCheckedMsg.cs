using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class BusHealthCheckedMsg : VportalEvent
  {
    public Guid ServiceId { get; set; }
  }
}
