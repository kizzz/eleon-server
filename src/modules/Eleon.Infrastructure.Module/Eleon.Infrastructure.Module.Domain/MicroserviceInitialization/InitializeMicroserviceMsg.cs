using Common.Module.Permissions;

namespace Messaging.Module.Messages
{
  // NOTE: Type conflict - DistributedEventAttribute exists in both Eleon.AbpSdk.Lib and Messaging.Module
  // The compiler will use the Messaging.Module version due to project reference order
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
