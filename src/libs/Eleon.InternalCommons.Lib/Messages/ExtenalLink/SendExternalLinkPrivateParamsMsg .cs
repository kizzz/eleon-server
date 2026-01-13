using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class SendExternalLinkPrivateParamsMsg : VportalEvent
  {
    public bool IsSuccess { get; set; }
    public string PrivateParams { get; set; }
  }
}
