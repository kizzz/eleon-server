using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class CancelAccountTenantMsg : VportalEvent
  {
    public string DocumentObjectType { get; set; }
    public string DocumentId { get; set; }

    public Guid? AccountTenantId { get; set; }
    public Guid? AccountId { get; set; }
  }
}
