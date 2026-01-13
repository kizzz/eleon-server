using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class NotificatorExecutedMsg : VportalEvent
  {
    public EleonsoftNotification Notification { get; set; }
  }
}
