using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class AddNotificationMsg : VportalEvent
  {
    public EleonsoftNotification Notification { get; set; }
  }
}
