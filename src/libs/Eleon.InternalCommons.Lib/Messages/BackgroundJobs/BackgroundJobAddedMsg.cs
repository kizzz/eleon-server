using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class BackgroundJobAddedMsg : VportalEvent
  {
    public BackgroundJobEto BackgroundJob { get; set; }
  }
}
