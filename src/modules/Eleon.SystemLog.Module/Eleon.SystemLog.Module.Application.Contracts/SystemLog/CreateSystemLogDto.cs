using Eleon.Logging.Lib.SystemLog.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.SystemLog.Module.SystemLog.Module.Application.Contracts.SystemLog;
public class CreateSystemLogDto
{
  public string Message { get; set; }
  public SystemLogLevel LogLevel { get; set; }
  public string ApplicationName { get; set; }
  public Dictionary<string, string> ExtraProperties { get; set; }
}
