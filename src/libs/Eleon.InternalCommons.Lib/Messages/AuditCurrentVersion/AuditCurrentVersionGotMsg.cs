using Common.Module.Constants;
using VPortal.Infrastructure.Module.Entities;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class AuditCurrentVersionGotMsg : VportalEvent
  {
    public DocumentVersionEntity CurrentVersion { get; set; }
  }
}
