using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.Auditor.Module.Auditor.Module.Application.Contracts.Audit;
public class GetVersionDto
{
  public string RefDocumentObjectType { get; set; }
  public string RefDocumentId { get; set; }
}
