using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class RequestBusHealthCheckMsg : VportalEvent
  {
    public List<Guid> ServiceIds { get; set; }
  }
}
