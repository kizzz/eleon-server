using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class GetBackgroundJobEtoMsg : VportalEvent
  {
    public Guid BackgroundJobId { get; set; }
  }
}
