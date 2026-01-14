using Eleon.Logging.Lib.SystemLog.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eleon.SystemServices.Module.Full.Eleon.SystemServicesModule.Module.Application.Contracts.SystemLog;

public class CreateSystemLogDto
{
  public string Message { get; set; }
  public SystemLogLevel LogLevel { get; set; }
  public string ApplicationName { get; set; }
  public Dictionary<string, string> ExtraProperties { get; set; }
}
