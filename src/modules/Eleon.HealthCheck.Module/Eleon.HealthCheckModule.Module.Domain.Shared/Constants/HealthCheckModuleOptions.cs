using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
public class HealthCheckModuleOptions
{
  public bool Enabled { get; set; } = false;
  public int IntervalMinutes { get; set; } = 30;
  public List<string> RequiredServices { get; set; } = new List<string>();
}
