using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.DocMessageLog.Module.DocMessageLogs;

namespace EleonsoftModuleCollector.SystemLog.Module.SystemLog.Module.Application.Contracts.SystemLog;

public class FullSystemLogDto : SystemLogDto
{
  public Dictionary<string, string> ExtraProperties { get; set; }
}
