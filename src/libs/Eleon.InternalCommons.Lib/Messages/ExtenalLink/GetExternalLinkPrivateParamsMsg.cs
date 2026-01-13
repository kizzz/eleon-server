using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class GetExternalLinkPrivateParamsMsg : VportalEvent
  {
    public string Password { get; set; }
    public string LinkCode { get; set; }
  }
}
