using EleonsoftSdk.modules.Messaging.Module.SystemMessages;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages;

public class StartingJobExecutionMsg : VportalEvent
{
  public BackgroundJobEto BackgroundJob { get; set; }
  public BackgroundJobExecutionEto Execution { get; set; }
}
