using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class AddNotificationsMsg : VportalEvent
  {
    public List<EleonsoftNotification> Notifications { get; set; }
  }
}
