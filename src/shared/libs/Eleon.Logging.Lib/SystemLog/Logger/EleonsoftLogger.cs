using Eleon.Logging.Lib.SystemLog.Contracts;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Logging.Lib.SystemLog.Logger;

internal sealed class EleonsoftLogger : ILogger
{
  private readonly string _category;
  private readonly EleonsoftLogOptions _opt;
  private readonly ILogger? _logger;
  private readonly Func<IExternalScopeProvider?> _getScopes;

  public EleonsoftLogger(string category, EleonsoftLogOptions opt, ILogger? fallbackLogger, Func<IExternalScopeProvider?> getScopes)
  { _category = category; _opt = opt; _logger = fallbackLogger; _getScopes = getScopes; }

  public IDisposable BeginScope<TState>(TState state)
  {
    var existingScope = _getScopes()?.Push(state);
    return existingScope ?? NullScope.Instance;
  }
  public bool IsEnabled(LogLevel logLevel)
  {
    // system log only Info+ (no Debug/Trace)
    if (logLevel == LogLevel.None || logLevel == LogLevel.Debug || logLevel == LogLevel.Trace || logLevel == LogLevel.Information) return false;

    return ToSystemLogLevel(logLevel) >= _opt.LogLevel;
  }

  public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
  {
    if (!IsEnabled(logLevel)) return;

    var msg = formatter(state, exception);

    // Collect structured state + scopes into extra props
    var props = new Dictionary<string, string>(StringComparer.Ordinal);
    props["Category"] = _category;
    props["EventId"] = eventId.Id.ToString();

    TryAddState(props, state);
    TryAddScopes(props, _getScopes());

    // simple re-entrancy guard (not a cross-provider duplicate guard)
    if (!EleonsoftBridgeGuard.Enter()) return;

    try
    {
      try
      {
        EleonsoftLog.Write(
            ToSystemLogLevel(logLevel),
            msg,
            exception,
            props);
      }
      catch (Exception ex)
      {
        _logger?.Log(LogLevel.Error, ex, "EleonsoftLog write error: {ErrorMessage}", ex.Message);
      }
      finally
      {
        // better to add parallel logging providers than to replace the entire logging system
        // _logger?.Log(logLevel, eventId, state, exception, formatter);
      }
    }
    finally { EleonsoftBridgeGuard.Exit(); }
  }

  private static void TryAddState<TState>(Dictionary<string, string> props, TState state)
  {
    if (state is IEnumerable<KeyValuePair<string, object?>> kvs)
    {
      foreach (var (k, v) in kvs)
        if (!string.IsNullOrEmpty(k) && v is not null)
          props[k] = v.ToString()!;
    }
    else
    {
      // fallback: capture state.ToString() if it's not the same as message
      var s = state?.ToString();
      if (!string.IsNullOrWhiteSpace(s)) props["State"] = s!;
    }
  }

  private static void TryAddScopes(Dictionary<string, string> props, IExternalScopeProvider? scopes)
  {
    scopes?.ForEachScope((scope, acc) =>
    {
      switch (scope)
      {
        case IEnumerable<KeyValuePair<string, object?>> kvs:
          foreach (var (k, v) in kvs)
            if (!string.IsNullOrEmpty(k) && v is not null)
              acc[k] = v.ToString()!;
          break;
        default:
          var s = scope?.ToString();
          if (!string.IsNullOrWhiteSpace(s)) acc["scope"] = s!;
          break;
      }
    }, props);
  }

  private static SystemLogLevel ToSystemLogLevel(LogLevel l) => l switch
  {
    LogLevel.Information => SystemLogLevel.Info,
    LogLevel.Warning => SystemLogLevel.Warning,
    LogLevel.Error => SystemLogLevel.Critical,
    LogLevel.Critical => SystemLogLevel.Critical,
    _ => SystemLogLevel.Info
  };

  private sealed class NullScope : IDisposable
  {
    public static readonly NullScope Instance = new();
    public void Dispose() { }
  }
}
internal static class EleonsoftBridgeGuard
{
  private static readonly AsyncLocal<bool> _inside = new();
  public static bool Enter()
  {
    if (_inside.Value) return false;
    _inside.Value = true;
    return true;
  }
  public static void Exit() => _inside.Value = false;
}
