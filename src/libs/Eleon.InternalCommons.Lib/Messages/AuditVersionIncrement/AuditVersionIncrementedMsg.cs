using Common.Module.Constants;
using VPortal.Infrastructure.Module.Entities;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class AuditVersionIncrementedMsg : VportalEvent
  {
    public DocumentVersionEntity NewVersion { get; set; }
    public bool Success { get; set; }
  }
}
