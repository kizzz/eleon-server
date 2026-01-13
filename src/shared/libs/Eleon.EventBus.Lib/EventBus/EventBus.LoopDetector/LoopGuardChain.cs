using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.EventBus.EventBus.LoopDetector;
public static class LoopGuardChain
{
  public static (List<string> entries, int hopCount) Parse(Headers headers)
  {
    var chain = headers.Get(LoopGuardHeaders.Chain, default(string));
    var list = string.IsNullOrWhiteSpace(chain)
        ? new List<string>()
        : chain.Split("->", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();

    var hops = headers.Get(LoopGuardHeaders.HopCount, default(int?)) ?? 0;
    return (list, hops);
  }

  public static string Entry(string serviceId, Type messageType) =>
      $"svc:{serviceId}|msg:{messageType.Name}";

  public static string TrimJoin(IEnumerable<string> entries, int max)
      => string.Join(" -> ", entries.TakeLast(max));
}
