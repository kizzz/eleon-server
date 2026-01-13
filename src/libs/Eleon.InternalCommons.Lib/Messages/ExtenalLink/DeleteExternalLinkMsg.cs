using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class DeleteExternalLinkMsg : VportalEvent
  {
    public string LinkCode { get; set; }
  }
}
