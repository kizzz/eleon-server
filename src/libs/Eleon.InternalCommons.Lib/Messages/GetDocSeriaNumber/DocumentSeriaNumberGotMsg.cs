using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class DocumentSeriaNumberGotMsg : VportalEvent
  {
    public string SeriaNumber { get; set; }
  }
}
