namespace Messaging.Module.Messages;

public class DocMessageLogCreatedMsg : VportalEvent
{
  public bool Success { get; set; }
  public Guid? DocMessageLogId { get; set; }
}
