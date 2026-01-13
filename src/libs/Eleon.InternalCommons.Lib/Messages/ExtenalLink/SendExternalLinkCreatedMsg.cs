using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class SendExternalLinkCreatedMsg : VportalEvent
  {
    public ExternalLinkEto ExternalLinkCreated { get; set; }
  }
}
