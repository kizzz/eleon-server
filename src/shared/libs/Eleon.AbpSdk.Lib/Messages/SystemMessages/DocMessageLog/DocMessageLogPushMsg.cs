using Messaging.Module.ETO;

namespace Messaging.Module.Messages;

public class DocMessageLogPushMsg : VportalEvent
{
  public DocMessageLogEto DocMessageLog { get; set; } = null!;
}
