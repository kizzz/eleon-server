using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenTelemetry.Logs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharedModule.modules.Otel.Module;

/// <summary>
/// Swappable logger provider that allows runtime replacement of the inner OpenTelemetryLoggerProvider
/// without accumulating providers in ILoggerFactory. This prevents memory leaks from multiple reconfigures.
/// </summary>
public sealed class SwappableLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private volatile OpenTelemetryLoggerProvider? _inner;
    private IExternalScopeProvider? _scopeProvider;

    public ILogger CreateLogger(string categoryName)
    {
        var inner = _inner;
        if (inner != null)
        {
            return inner.CreateLogger(categoryName);
        }

        // Return a no-op logger if inner provider is not set
        return NullLogger.Instance;
    }

    public void Dispose()
    {
        var old = Interlocked.Exchange(ref _inner, null);
        old?.Dispose();
    }

    /// <summary>
    /// Swaps the inner OpenTelemetryLoggerProvider with a new one, disposing the old one.
    /// This method is thread-safe.
    /// </summary>
    public void Swap(OpenTelemetryLoggerProvider? next)
    {
        var old = Interlocked.Exchange(ref _inner, next);
        
        // Set scope provider on the new inner provider if it supports it
        if (next is ISupportExternalScope nextScopeSupport && _scopeProvider != null)
        {
            nextScopeSupport.SetScopeProvider(_scopeProvider);
        }
        
        // Dispose the old provider (in background to avoid blocking)
        if (old != null)
        {
            Task.Run(() =>
            {
                try
                {
                    old.Dispose();
                }
                catch
                {
                    // Silently ignore disposal errors
                }
            });
        }
    }

    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;
        
        // If inner provider supports external scope, set it there too
        if (_inner is ISupportExternalScope innerScopeSupport)
        {
            innerScopeSupport.SetScopeProvider(scopeProvider);
        }
    }
}
