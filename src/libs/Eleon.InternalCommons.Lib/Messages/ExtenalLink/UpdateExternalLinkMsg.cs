using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class UpdateExternalLinkMsg : VportalEvent
  {
    public ExternalLinkEto UpdateExternalLinkEto { get; set; }
  }
}
