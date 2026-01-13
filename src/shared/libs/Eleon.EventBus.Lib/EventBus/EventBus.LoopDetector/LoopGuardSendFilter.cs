using MassTransit;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace EleonsoftSdk.modules.EventBus.EventBus.LoopDetector;
public class LoopGuardSendFilter<T> : IFilter<SendContext<T>> where T : class
{
  private readonly LoopGuardOptions _opt;

  public LoopGuardSendFilter(IOptions<LoopGuardOptions> opt) => _opt = opt.Value;

  public void Probe(ProbeContext context) { }

  public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
  {
    // 1) RootRequestId
    var root = context.Headers.Get(LoopGuardHeaders.RootRequestId, default(string));
    if (string.IsNullOrWhiteSpace(root))
      root = Guid.NewGuid().ToString("N");
    context.Headers.Set(LoopGuardHeaders.RootRequestId, root);

    // 2) Traceparent (OpenTelemetry)
    var traceParent = Activity.Current?.Id; // MassTransit also propagates OTel if configured
    if (!string.IsNullOrEmpty(traceParent))
      context.Headers.Set(LoopGuardHeaders.TraceId, traceParent);

    // 3) Deadline / Time budget
    var deadlineStr = context.Headers.Get(LoopGuardHeaders.DeadlineUtc, default(string));
    if (!DateTimeOffset.TryParse(deadlineStr, out var deadline))
    {
      deadline = DateTimeOffset.UtcNow + _opt.DefaultTimeBudget;
    }
    // Ensure non-increasing deadline (never extend)
    var now = DateTimeOffset.UtcNow;
    if (deadline <= now)
      throw new InvalidOperationException("LoopGuard: Deadline already expired before send.");

    context.Headers.Set(LoopGuardHeaders.DeadlineUtc, deadline.UtcDateTime.ToString("o"));

    // Optional: let the transport drop late messages too
    context.TimeToLive = deadline - now;

    // 4) Hop count (increment only for Send/Request; optionally for Publish)
    var (entries, hops) = LoopGuardChain.Parse(context.Headers);
    var isPublish = context.DestinationAddress?.AbsolutePath?.Contains("exchange") == true; // heuristic for RabbitMQ
    var shouldIncrement = !isPublish || _opt.TrackOnPublish;

    var newHops = shouldIncrement ? hops + 1 : hops;
    context.Headers.Set(LoopGuardHeaders.HopCount, newHops);

    // 5) Chain (append current entry)
    entries.Add(LoopGuardChain.Entry(_opt.ServiceId, typeof(T)));
    context.Headers.Set(LoopGuardHeaders.Chain, LoopGuardChain.TrimJoin(entries, _opt.MaxChainEntries));

    // 6) Version
    context.Headers.Set(LoopGuardHeaders.Version, "1");

    return next.Send(context);
  }
}
