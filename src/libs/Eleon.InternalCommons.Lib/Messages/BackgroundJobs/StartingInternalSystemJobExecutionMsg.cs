using Common.Module.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class StartingInternalSystemJobExecutionMsg : VportalEvent
  {
    public BackgroundJobEto BackgroundJob { get; set; }
    public BackgroundJobExecutionEto Execution { get; set; }
  }
}
