using Common.Module.Constants;
using VPortal.Infrastructure.Module.Entities;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class IncrementAuditDocumentVersionMsg : VportalEvent
  {
    public string AuditedDocumentObjectType { get; set; }
    public string AuditedDocumentId { get; set; }
    public DocumentVersionEntity Version { get; set; }

  }
}
