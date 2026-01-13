using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class SendBulkNotificationMsg : VportalEvent
  {
    public List<BackgroundJobEto> Jobs { get; set; }
  }
}
