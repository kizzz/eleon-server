using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class AllTenantsGotMsg : VportalEvent
  {
    public List<TenantEto> Tenants { get; set; }
  }
}
