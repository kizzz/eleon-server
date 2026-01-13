using VPortal.Infrastructure.Module.Entities;

namespace ModuleCollector.Auditor.Module.Auditor.Module.Application.Contracts.Audit;
public class AuditDto
{
  public string Data { get; set; }
  public DocumentVersionEntity DocumentVersion { get; set; }

  public AuditDto()
  {

  }

  public AuditDto(string data, DocumentVersionEntity documentVersion)
  {
    Data = data;
    DocumentVersion = documentVersion;
  }
}
