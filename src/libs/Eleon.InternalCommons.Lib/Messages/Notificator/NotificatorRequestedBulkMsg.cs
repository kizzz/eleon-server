using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class NotificatorRequestedBulkMsg : VportalEvent
  {
    public List<NotificatorRequestedMsg> Messages { get; set; } = new List<NotificatorRequestedMsg>();
  }
}
