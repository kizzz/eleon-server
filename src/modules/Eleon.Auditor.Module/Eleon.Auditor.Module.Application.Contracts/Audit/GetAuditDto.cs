using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.Auditor.Module.Auditor.Module.Application.Contracts.Audit;
public class GetAuditDto
{
  public string AuditedDocumentObjectType { get; set; }
  public string AuditedDocumentId { get; set; }
  public string Version { get; set; }
}
