using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.EventBus.EventBus.LoopDetector;
public class LoopGuardOptions
{
  public int MaxHops { get; set; } = 4;
  public int MaxChainEntries { get; set; } = 16;
  public TimeSpan DefaultTimeBudget { get; set; } = TimeSpan.FromSeconds(30);
  public bool TrackOnPublish { get; set; } = false; // usually false
  public string ServiceId { get; set; } = "Default";
}
