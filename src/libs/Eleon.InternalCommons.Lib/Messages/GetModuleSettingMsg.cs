using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class GetModuleSettingMsg : VportalEvent
  {
    public new Guid? TenantId { get; set; }
  }
}
