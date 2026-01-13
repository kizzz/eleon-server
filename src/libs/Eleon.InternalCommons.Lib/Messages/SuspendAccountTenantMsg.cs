using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class SuspendAccountTenantMsg : VportalEvent
  {
    public string ObjectType { get; set; }
    public string DocumentId { get; set; }

    public Guid? AccountTenantId { get; set; }
    public Guid? AccountId { get; set; }
  }
}
