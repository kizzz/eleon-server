using VPortal.Infrastructure.Module.Entities;

namespace ModuleCollector.Auditor.Module.Auditor.Module.Application.Contracts.Audit;
public class IncrementVersionResultDto
{
  public bool Success { get; set; }
  public DocumentVersionEntity NewDocumentVersion { get; set; }
}
