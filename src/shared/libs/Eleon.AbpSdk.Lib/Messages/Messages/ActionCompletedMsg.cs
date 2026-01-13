using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class ActionCompletedMsg : VportalEvent
  {
    public bool Success { get; set; }
    public string Error { get; set; }
  }
}
