using MassTransit;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.EventBus.EventBus.LoopDetector;
public class LoopGuardConsumeFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
  private readonly LoopGuardOptions _opt;

  public LoopGuardConsumeFilter(IOptions<LoopGuardOptions> opt) => _opt = opt.Value;

  public void Probe(ProbeContext context) { }

  public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
  {
    var now = DateTimeOffset.UtcNow;

    // Normalize mandatory headers
    var root = context.Headers.Get(LoopGuardHeaders.RootRequestId, default(string))
               ?? Guid.NewGuid().ToString("N");
    //context.Headers.Set(LoopGuardHeaders.RootRequestId, root);

    // Deadline
    var deadlineStr = context.Headers.Get(LoopGuardHeaders.DeadlineUtc, default(string));
    if (!DateTimeOffset.TryParse(deadlineStr, out var deadline))
    {
      deadline = now + _opt.DefaultTimeBudget; // if missing, start a fresh budget
                                               //context.Headers.Set(LoopGuardHeaders.DeadlineUtc, deadline.UtcDateTime.ToString("o"));
    }
    if (deadline <= now)
      throw new LoopDetectedException("LoopGuard: Deadline exceeded on consume.", context);

    // Hop count
    var hops = context.Headers.Get(LoopGuardHeaders.HopCount, default(int?)) ?? 0;
    if (hops > _opt.MaxHops)
      throw new LoopDetectedException($"LoopGuard: Max hop count exceeded ({hops}>{_opt.MaxHops}).", context);

    // Chain revisit detection
    var chainStr = context.Headers.Get(LoopGuardHeaders.Chain, default(string));
    var chain = string.IsNullOrWhiteSpace(chainStr)
        ? new List<string>()
        : chainStr.Split("->", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();

    var currentEntry = LoopGuardChain.Entry(_opt.ServiceId, typeof(T));
    var seen = chain.Count > 0 && chain.Any(e => e.Equals(currentEntry, StringComparison.Ordinal));

    if (seen)
      throw new LoopDetectedException($"LoopGuard: Revisited {currentEntry} in chain (cycle).", context);

    // Logging scope (great for Kibana/Seq)
    using (Serilog.Context.LogContext.PushProperty("lg_root", root))
    using (Serilog.Context.LogContext.PushProperty("lg_hops", hops))
    using (Serilog.Context.LogContext.PushProperty("lg_deadline", deadline))
    using (Serilog.Context.LogContext.PushProperty("lg_chain", chainStr))
    using (Serilog.Context.LogContext.PushProperty("msg_type", typeof(T).Name))
    {
      await next.Send(context);
    }
  }
}

public sealed class LoopDetectedException : Exception
{
  public string? RootRequestId { get; }
  public string? Chain { get; }
  public int? HopCount { get; }
  public DateTimeOffset? Deadline { get; }

  public LoopDetectedException(string message, ConsumeContext ctx) : base(Build(message, ctx))
  {
    RootRequestId = ctx.Headers.Get(LoopGuardHeaders.RootRequestId, default(string));
    Chain = ctx.Headers.Get(LoopGuardHeaders.Chain, default(string));
    HopCount = ctx.Headers.Get(LoopGuardHeaders.HopCount, default(int?));
    if (DateTimeOffset.TryParse(ctx.Headers.Get(LoopGuardHeaders.DeadlineUtc, default(string)), out var d))
      Deadline = d;
  }

  static string Build(string message, ConsumeContext ctx)
  {
    var root = ctx.Headers.Get(LoopGuardHeaders.RootRequestId, default(string));
    var hops = ctx.Headers.Get(LoopGuardHeaders.HopCount, default(int?)) ?? 0;
    var chain = ctx.Headers.Get(LoopGuardHeaders.Chain, default(string));
    var dl = ctx.Headers.Get(LoopGuardHeaders.DeadlineUtc, default(string));
    return $"{message} root={root}, hops={hops}, deadline={dl}, chain={chain}";
  }
}
