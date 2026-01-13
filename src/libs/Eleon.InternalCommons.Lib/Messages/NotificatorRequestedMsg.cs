using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class NotificatorRequestedMsg : VportalEvent
  {
    public EleonsoftNotification Notification { get; set; }
  }
}
