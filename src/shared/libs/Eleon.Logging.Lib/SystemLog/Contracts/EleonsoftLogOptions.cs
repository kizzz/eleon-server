using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Logging.Lib.SystemLog.Contracts;

public sealed class EleonsoftLogOptions
{
  // Level filtering for the whole pipeline
  public SystemLogLevel LogLevel { get; set; } = SystemLogLevel.Info;

  // Batching & queueing
  public int BatchSize { get; set; } = 64;
  public int BatchIntervalMs { get; set; } = 800;
  // Defaults (auto-filled if individual entries omit them)
  public string? DefaultApplicationName { get; set; }

  // Time formatting for SystemLogEntry.Time (default: ISO-8601 "O")
  public Func<DateTimeOffset>? NowProvider { get; set; } = () => DateTimeOffset.UtcNow;

  // Forward logs to Serilog (disabled by default to prevent blocking during early startup)
  // Set to true after Serilog is configured
  public bool ForwardToSerilog { get; set; } = false;
}
