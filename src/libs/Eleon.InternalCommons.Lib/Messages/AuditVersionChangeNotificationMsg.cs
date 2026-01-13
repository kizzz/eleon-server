using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class AuditVersionChangeNotificationMsg : VportalEvent
  {
    public AuditVersionChangeNotificationEto AuditChange { get; set; }
  }
}
