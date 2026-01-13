using Eleon.Logging.Lib.SystemLog.Contracts;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Logging.Lib.SystemLog.Enrichers;

public sealed class HostInfoEnricher : ISystemLogEnricher
{
  private readonly string _host = Environment.MachineName;
  private readonly int _pid = Environment.ProcessId;

  public void Enrich(Dictionary<string, string> entry)
  {
    if (!entry.ContainsKey("MachineName"))
    {
      entry.Add("MachineName", _host);
    }
    if (!entry.ContainsKey("Pid"))
    {
      entry.Add("Pid", _pid.ToString());
    }
  }
}
