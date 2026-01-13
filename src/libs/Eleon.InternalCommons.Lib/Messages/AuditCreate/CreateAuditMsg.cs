using Common.Module.Constants;
using VPortal.Infrastructure.Module.Entities;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class CreateAuditMsg : VportalEvent
  {
    public string RefDocumentObjectType { get; set; }
    public string RefDocumentId { get; set; }
    public string AuditedDocumentObjectType { get; set; }
    public string AuditedDocumentId { get; set; }
    public DocumentVersionEntity DocumentVersion { get; set; }
    public string DocumentData { get; set; }
  }
}
