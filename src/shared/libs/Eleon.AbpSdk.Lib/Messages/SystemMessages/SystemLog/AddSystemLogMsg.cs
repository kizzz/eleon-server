using Common.Module.Events;
using Eleon.Logging.Lib.SystemLog.Contracts;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.Commons.Module.Messages.SystemLog;

[DistributedEvent]
public class AddSystemLogMsg : VportalEvent
{
  public List<AddSystemLogEto> Logs { get; set; } = new List<AddSystemLogEto>();
}

public class AddSystemLogEto
{
  public string Message { get; set; }
  public SystemLogLevel LogLevel { get; set; }
  public string ApplicationName { get; set; }
  public Dictionary<string, object> ExtraProperties { get; set; } = new Dictionary<string, object>();
}
