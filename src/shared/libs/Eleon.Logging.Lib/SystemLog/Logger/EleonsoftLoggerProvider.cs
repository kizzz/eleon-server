using Eleon.Logging.Lib.SystemLog.Contracts;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Logging.Lib.SystemLog.Logger;

public sealed class EleonsoftLoggerProvider : ILoggerProvider, ISupportExternalScope
{
  private readonly EleonsoftLogOptions _opt;
  private IExternalScopeProvider? _scopes;
  private readonly ConcurrentDictionary<string, EleonsoftLogger> _cache = new ConcurrentDictionary<string, EleonsoftLogger>();
  private readonly ILoggerFactory _fallbackFactory;

  public EleonsoftLoggerProvider(EleonsoftLogOptions? options = null, ILoggerFactory? fallbackFactory = null)
  {
    _opt = options ?? new EleonsoftLogOptions();
    _fallbackFactory = fallbackFactory ?? LoggerFactory.Create(b =>
    {
      b.AddSerilog(Log.Logger, dispose: false);
    });
  }

  private IExternalScopeProvider EnsureScopes()
      => Volatile.Read(ref _scopes) ?? Interlocked.CompareExchange(ref _scopes, new LoggerExternalScopeProvider(), null) ?? _scopes!;

  public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
      => _cache.GetOrAdd(categoryName, cat => new EleonsoftLogger(cat, _opt, _fallbackFactory.CreateLogger(categoryName), () => EnsureScopes()));

  public void Dispose() => _cache.Clear();

  public void SetScopeProvider(IExternalScopeProvider scopeProvider) => _scopes = scopeProvider ?? new LoggerExternalScopeProvider();
}
