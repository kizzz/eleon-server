using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class SetMicroserviceConnectionStringMsg : VportalEvent
  {
    public Guid ServiceId { get; set; }
    public string ConnectionString { get; set; }
  }
}
