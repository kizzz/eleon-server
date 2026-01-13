using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.SystemCheck;
public class EnvironmentHealthCheckOptions
{
  public double CpuThresholdPercent { get; set; } = 95.0;
  public double MemoryThresholdPercent { get; set; } = 95.0;
  public double DiskThresholdPercent { get; set; } = 95.0;
  public bool FailOnDiskThreshold { get; set; } = false;
  public int CpuSampleMilliseconds { get; set; } = 1000;
}
