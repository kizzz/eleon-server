using Eleon.Logging.Lib.VportalLogging;
using Logging.Module;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Volo.Abp.MultiTenancy;

namespace Eleon.Logging.Lib.VportalLogger
{
  public class VPortalLogger<T> : IVportalLogger<T>
  {
    private readonly ILogger<T> _logger;
    private readonly IExceptionReporter _exceptionReporter;
    private readonly string _componentName;
    private readonly string _tenantValue;

    public VPortalLogger(ILogger<T> logger, IExceptionReporter exceptionReporter, ICurrentTenant? currentTenant = null)
    {
      _logger = logger;
      _exceptionReporter = exceptionReporter;
      _componentName = typeof(T).Name;
      var tenantId = currentTenant?.Id?.ToString();
      _tenantValue = string.IsNullOrWhiteSpace(tenantId) ? "Host" : tenantId;
    }

    public ILogger Log => _logger;

    [DoesNotReturn]
    public void Capture(Exception ex, [CallerMemberName] string memberName = "")
    {
      CaptureAndSuppress(ex, memberName);
      ExceptionDispatchInfo.Throw(ex);
    }

    public void CaptureAndSuppress(Exception ex, [CallerMemberName] string memberName = "")
    {
      _exceptionReporter.Report(ex, BuildContext(memberName));
    }

    /// <summary>
    /// No-op; kept for temporary compatibility. Do not use.
    /// </summary>
    [Obsolete("VportalLogger.LogStart/LogFinish are deprecated. Use boundary logging (HTTP middleware / SignalR filter / job & consumer boundary scopes).", false)]
    public void LogFinish([CallerMemberName] string memberName = "")
    {
    }

    /// <summary>
    /// No-op; kept for temporary compatibility. Do not use.
    /// </summary>
    [Obsolete("VportalLogger.LogHttpMethodStart/LogHttpMethodFinish are deprecated. Use boundary logging (HTTP middleware / SignalR filter / job & consumer boundary scopes).", false)]
    public void LogHttpMethodFinish([CallerMemberName] string memberName = "")
    {
    }

    /// <summary>
    /// No-op; kept for temporary compatibility. Do not use.
    /// </summary>
    [Obsolete("VportalLogger.LogHttpMethodStart/LogHttpMethodFinish are deprecated. Use boundary logging (HTTP middleware / SignalR filter / job & consumer boundary scopes).", false)]
    public void LogHttpMethodStart([CallerMemberName] string memberName = "")
    {
    }

    /// <summary>
    /// No-op; kept for temporary compatibility. Do not use.
    /// </summary>
    [Obsolete("VportalLogger.LogStart/LogFinish are deprecated. Use boundary logging (HTTP middleware / SignalR filter / job & consumer boundary scopes).", false)]
    public void LogStart(object argsInfo = null, [CallerMemberName] string memberName = "")
    {
    }

    private IDisposable BeginScope(string memberName)
    {
      return _logger.BeginScope(BuildContext(memberName));
    }

    private IReadOnlyDictionary<string, object?> BuildContext(string memberName)
    {
      var context = new Dictionary<string, object?>(StringComparer.Ordinal)
      {
        [VportalLogProperties.Component] = _componentName,
        [VportalLogProperties.Operation] = memberName,
        [VportalLogProperties.Tenant] = _tenantValue
      };

      var activity = Activity.Current;
      if (activity != null)
      {
        context[VportalLogProperties.TraceId] = activity.TraceId.ToString();
        context[VportalLogProperties.SpanId] = activity.SpanId.ToString();
      }

      return context;
    }
  }
}
