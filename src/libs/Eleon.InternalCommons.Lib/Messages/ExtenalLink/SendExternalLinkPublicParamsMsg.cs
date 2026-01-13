using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class SendExternalLinkPublicParamsMsg : VportalEvent
  {
    public bool IsSuccess { get; set; }
    public string PublicParams { get; set; }
  }
}
