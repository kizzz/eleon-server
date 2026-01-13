using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.DocMessageLog.Module.DocMessageLogs;
public class UnresolvedSystemLogCountDto
{
  public int CriticalUnresolvedCount { get; set; }
  public int WarningUnresolvedCount { get; set; }
}
