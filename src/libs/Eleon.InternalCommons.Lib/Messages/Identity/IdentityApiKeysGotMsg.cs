using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class IdentityApiKeysGotMsg : VportalEvent
  {
    public List<IdentityApiKeyEto> Keys { get; set; }
  }
}
