using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class LifecycleRequestedMsg : VportalEvent
  {
    public Guid TemplateId { get; set; }
    public string DocumentId { get; set; }
    public string DocEntry { get; set; }
    public Guid InitiatorId { get; set; }
    public string DocumentObjectType { get; set; }
    public bool StartImmediately { get; set; }
    public bool IsSkipFilled { get; set; }

    // TODO Extra Properties
  }
}
