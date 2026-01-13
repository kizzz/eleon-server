using VPortal.Infrastructure.Module.Entities;

namespace Messaging.Module.ETO
{
  public class AuditedDocumentEto
  {
    public string Data { get; set; }
    public DocumentVersionEntity Version { get; set; }
  }
}
