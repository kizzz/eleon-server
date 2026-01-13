using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class BackgroundJobExecutedMsg : VportalEvent
  {
    public BackgroundJobEto BackgroundJob { get; set; }
  }
}
