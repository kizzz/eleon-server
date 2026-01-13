using MassTransit;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.EventBus.EventBus.LoopDetector;

public class LoopGuardPublishFilter<T> : IFilter<PublishContext<T>> where T : class
{
  private readonly LoopGuardOptions _opt;
  public LoopGuardPublishFilter(IOptions<LoopGuardOptions> opt) => _opt = opt.Value;

  public void Probe(ProbeContext context) { }

  public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
  {
    // set/normalize headers for PUBLISH (events)
    var hops = context.Headers.Get("lg-hop-count", default(int?)) ?? 0;
    // If you don’t want to count publishes, skip increment here.
    context.Headers.Set("lg-hop-count", hops + 1);

    var chain = context.Headers.Get("lg-chain", default(string)) ?? string.Empty;
    var entry = $"svc:{_opt.ServiceId}|msg:{typeof(T).Name}";
    context.Headers.Set("lg-chain", string.IsNullOrEmpty(chain) ? entry : $"{chain} -> {entry}");

    // optional: propagate deadline/TTL
    var now = DateTimeOffset.UtcNow;
    var deadlineStr = context.Headers.Get("lg-deadline-utc", default(string));
    if (DateTimeOffset.TryParse(deadlineStr, out var deadline) && deadline > now)
      context.TimeToLive = deadline - now;

    return next.Send(context);
  }
}
