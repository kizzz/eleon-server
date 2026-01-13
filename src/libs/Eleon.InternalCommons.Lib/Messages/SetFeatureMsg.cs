using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class SetFeatureMsg : VportalEvent
  {
    public Guid FeatureTenantId { get; set; }
    public string FeatureName { get; set; }
    public string FeatureValue { get; set; }
  }
}
