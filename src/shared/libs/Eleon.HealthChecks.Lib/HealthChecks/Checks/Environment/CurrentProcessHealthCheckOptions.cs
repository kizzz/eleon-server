using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.SystemCheck;
public class CurrentProcessHealthCheckOptions
{
  /// <summary>Fail if current process CPU exceeds this percent (sampled over CpuSampleMilliseconds).</summary>
  public double CpuThresholdPercent { get; set; } = 95.0;

  /// <summary>Fail if Working Set exceeds this many MB.</summary>
  public double WorkingSetThresholdMB { get; set; } = 4096; // 4 GB default

  /// <summary>Fail if GC heap size exceeds this many MB.</summary>
  public double HeapThresholdMB { get; set; } = 4096; // 4 GB default

  /// <summary>Fail if Thread count exceeds this number.</summary>
  public int ThreadThreshold { get; set; } = 1000;

  /// <summary>CPU sampling window in milliseconds.</summary>
  public int CpuSampleMilliseconds { get; set; } = 1000;
}
