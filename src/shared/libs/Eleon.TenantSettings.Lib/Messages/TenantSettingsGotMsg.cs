using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class TenantSettingsGotMsg : VportalEvent
  {
    public string SettingsJson { get; set; }
  }
}
