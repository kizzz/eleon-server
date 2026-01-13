using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class AccountTenantActionMsg : VportalEvent
  {
    public Guid? AccountId { get; set; }
    public Guid? AccountTenantId { get; set; }
    public string ErrorMsg { get; set; }
  }
}
