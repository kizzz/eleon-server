using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class SetFeatureSettingMsg : VportalEvent
  {
    public Guid? SettingTenantId { get; set; }
    public string SettingGroup { get; set; }
    public string SettingKey { get; set; }
    public string Value { get; set; }
    public string Type { get; set; }
  }
}
