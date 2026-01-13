using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.Infrastructure.Module.Entities;

namespace ModuleCollector.Auditor.Module.Auditor.Module.Application.Contracts.Audit
{
  public class CreateAuditDto
  {
    public string RefDocumentObjectType { get; set; }
    public string RefDocumentId { get; set; }
    public string AuditedDocumentObjectType { get; set; }
    public string AuditedDocumentId { get; set; }
    public string DocumentData { get; set; }
    public DocumentVersionEntity DocumentVersion { get; set; }
  }
}
