using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class BackgroundJobEtoGotMsg : VportalEvent
  {
    public bool Success { get; set; }
    public BackgroundJobEto? BackgroundJob { get; set; }
  }
}
