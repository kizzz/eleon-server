using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.HttpCheck;
public class HttpHealthCheckOptions
{
  public int Timeout { get; set; } = 40;
  public bool IgnoreSsl { get; set; } = false;
  public List<HealthCheckUrl> Urls { get; set; } = new List<HealthCheckUrl>();
}

public class HealthCheckUrl
{
  public string Name { get; set; }
  public string Url { get; set; }
  public List<int> GoodStatusCodes { get; set; } = new List<int> { };
}
