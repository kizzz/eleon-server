using Messaging.Module.ETO;

namespace Messaging.Module.Messages;

public class DocMessageLogRetryMsg : VportalEvent
{
  public required DocMessageLogEto DocMessageLog { get; set; }
}
