using Eleon.Logging.Lib.SystemLog.Contracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eleon.Logging.Lib.SystemLog.Sinks
{
  /// <summary>
  /// Minimal console sink: one pretty text line per entry, fixed UTC timestamp, colors:
  /// - INFO: default color
  /// - WARN: DarkYellow (orange-ish)
  /// - CRIT: Red
  /// Errors go to stderr, others to stdout.
  /// </summary>
  public sealed class ConsoleSystemLogSink : ISystemLogSink, IDisposable
  {
    private static readonly object _lock = new();
    private volatile bool _disposed;

    public Task WriteAsync(IReadOnlyList<SystemLogEntry> batch, CancellationToken ct)
    {
      if (_disposed || batch is null || batch.Count == 0) return Task.CompletedTask;

      foreach (var e in batch)
      {
        if (ct.IsCancellationRequested) break;
        WriteOne(e);
      }

      return Task.CompletedTask;
    }

    private static void WriteOne(SystemLogEntry e)
    {
      bool toError = e.LogLevel >= SystemLogLevel.Critical;

      // Build one compact line
      var sb = new StringBuilder(256);
      sb.Append('[').Append(Ts(e.Time.ToString())).Append("] ");
      sb.Append(Lvl4(e.LogLevel)).Append(' ');

      if (!string.IsNullOrWhiteSpace(e.ApplicationName))
        sb.Append(e.ApplicationName).Append(' ');

      if (!string.IsNullOrEmpty(e.ExtraProperties?.GetValueOrDefault("TenantId") ?? string.Empty))
        sb.Append("tenant=").Append(e.ExtraProperties?.GetValueOrDefault("TenantId") ?? string.Empty).Append(' ');

      if (!string.IsNullOrWhiteSpace(e.Message))
        sb.Append("- ").Append(e.Message.Trim()).Append(' ');

      if (e.ExtraProperties is { Count: > 0 })
        sb.Append("| props{").Append(Props(e.ExtraProperties)).Append("} ");

      if (e.Exception is not null)
        sb.Append("| ex=").Append(e.Exception!.ToString().Trim());

      var line = sb.ToString().TrimEnd();

      lock (_lock)
      {
        var original = Console.ForegroundColor;
        try
        {
          if (e.LogLevel >= SystemLogLevel.Critical)
            Console.ForegroundColor = ConsoleColor.Red;
          else if (e.LogLevel >= SystemLogLevel.Warning)
            Console.ForegroundColor = ConsoleColor.DarkYellow;

          if (toError) Console.Error.WriteLine(line);
          else Console.Out.WriteLine(line);
        }
        finally
        {
          Console.ForegroundColor = original;
        }
      }
    }

    private static string Ts(string? original)
    {
      // Always output UTC ISO 8601 with 'Z'
      if (!string.IsNullOrWhiteSpace(original) && DateTimeOffset.TryParse(original, out var dto))
        return dto.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");
      return DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");
    }

    private static string Lvl4(SystemLogLevel lvl) => lvl switch
    {
      SystemLogLevel.Critical => "CRIT",
      SystemLogLevel.Warning => "WARN",
      _ => "INFO"
    };

    private static string Props(Dictionary<string, string> props)
    {
      // stable, compact: k=v pairs separated by ", ", ordered by key
      return string.Join(", ",
          props.OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase)
               .Select(p => $"{p.Key}={Safe(p.Value)}"));
    }

    private static string Safe(string? s)
        => string.IsNullOrEmpty(s) ? "" : s.Replace('\r', ' ').Replace('\n', ' ').Trim();

    public void Dispose() => _disposed = true;
  }
}
