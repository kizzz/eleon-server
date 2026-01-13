using Common.Module.Constants;
using VPortal.Infrastructure.Module.Entities;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class AuditCreatedMsg : VportalEvent
  {
    public bool CreatedSuccessfully { get; set; }
    public DocumentVersionEntity DocumentVersion { get; set; }
  }
}
