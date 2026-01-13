using Common.Module.Constants;
using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Auditor.Module.Hubs
{
  public class AuditVersionChangeNotificationDto
  {
    public string DocumentObjectType { get; set; }
    public string DocumentId { get; set; }
    public DocumentVersionEntity NewVersion { get; set; }
  }
}
