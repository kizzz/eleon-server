using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class UserRemovedMsg : VportalEvent
  {
    public Guid UserId { get; set; }
  }
}
