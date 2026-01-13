using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class GetAuditDocumentMsg : VportalEvent
  {
    public string AuditedDocumentObjectType { get; set; }
    public string AuditedDocumentId { get; set; }
    public string Version { get; set; }
  }
}
