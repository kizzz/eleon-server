using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class AuditDocumentGotMsg : VportalEvent
  {
    public AuditedDocumentEto AuditedDocument { get; set; }
  }
}
