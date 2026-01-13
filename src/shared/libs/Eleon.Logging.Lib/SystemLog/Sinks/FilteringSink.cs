using Eleon.Logging.Lib.SystemLog.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Logging.Lib.SystemLog.Sinks;

public sealed class FilteringSink : ISystemLogSink
{
  private readonly ISystemLogSink _inner;
  private readonly SystemLogLevel _min;

  public FilteringSink(ISystemLogSink inner, SystemLogLevel minLevel)
  {
    _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    _min = minLevel;
  }

  public Task WriteAsync(IReadOnlyList<SystemLogEntry> batch, CancellationToken ct)
  {
    var filtered = batch.Where(e => e.LogLevel >= _min).ToArray();
    return filtered.Length > 0 ? _inner.WriteAsync(filtered, ct) : Task.CompletedTask;
  }

  public ISystemLogSink Sink => _inner;
}
