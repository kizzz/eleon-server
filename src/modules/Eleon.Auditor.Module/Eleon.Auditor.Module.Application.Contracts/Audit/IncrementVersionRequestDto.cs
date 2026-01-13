using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.Infrastructure.Module.Entities;

namespace ModuleCollector.Auditor.Module.Auditor.Module.Application.Contracts.Audit;
public class IncrementVersionRequestDto
{
  public string AuditedDocumentObjectType { get; set; }

  public string AuditedDocumentId { get; set; }

  public DocumentVersionEntity? Version { get; set; }
}
