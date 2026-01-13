using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.SystemLog.Module.SystemLog.Module.Application.Contracts.SystemLog;
public class MarkSystemLogsReadedRequestDto
{
  public List<Guid> LogIds { get; set; }
  public bool IsReaded { get; set; }
}
