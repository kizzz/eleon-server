using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class GetAuditCurrentVersionMsg : VportalEvent
  {
    public string RefDocumentObjectType { get; set; }
    public string RefDocumentId { get; set; }
  }
}
