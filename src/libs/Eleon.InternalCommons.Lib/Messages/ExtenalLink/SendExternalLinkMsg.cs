using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class SendExternalLinkMsg : VportalEvent
  {
    public ExternalLinkEto ExternalLinkEto { get; set; }
  }
}
