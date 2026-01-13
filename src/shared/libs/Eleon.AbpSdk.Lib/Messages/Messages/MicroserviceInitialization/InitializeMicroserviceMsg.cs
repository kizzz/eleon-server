using Common.Module.Permissions;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class InitializeMicroserviceMsg : VportalEvent
  {
    public Guid? RequestId { get; set; }

    public MicroserviceInfoEto Info { get; set; }
  }

  public class MicroserviceInfoEto
  {
    public Guid ServiceId { get; set; }
    public string DisplayName { get; set; }
    public List<FeatureGroupDescription> Features { get; set; }
  }
}
