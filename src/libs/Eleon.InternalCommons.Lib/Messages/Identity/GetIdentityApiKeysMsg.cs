using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class GetIdentityApiKeysMsg : VportalEvent
  {
    public List<ApiKeyType> TypesFilter { get; set; }
  }
}
