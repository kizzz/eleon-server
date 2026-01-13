using Common.Module.Constants;
using VPortal.Infrastructure.Module.Entities;

namespace Messaging.Module.ETO
{
  public class AuditVersionChangeNotificationEto
  {
    public string DocumentObjectType { get; set; }
    public string DocumentId { get; set; }
    public DocumentVersionEntity NewVersion { get; set; }
  }
}
