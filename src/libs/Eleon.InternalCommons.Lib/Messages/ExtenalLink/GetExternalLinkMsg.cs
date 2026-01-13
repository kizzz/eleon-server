using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class GetExternalLinkMsg : VportalEvent
  {
    public string LinkCode { get; set; }
  }
}
