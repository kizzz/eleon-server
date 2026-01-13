using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class RequestMicroserviceConnectionStringMsg : VportalEvent
  {
    public Guid ServiceId { get; set; }
  }
}
