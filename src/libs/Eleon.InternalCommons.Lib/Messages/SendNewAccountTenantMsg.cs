using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class SendNewAccountTenantMsg : VportalEvent
  {
    public Guid? NewAccountTenantId { get; set; }
  }
}
