using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sentry;
using System.Diagnostics;

namespace Eleon.Logging.Lib.VportalLogging;

public sealed class VportalExceptionReporter : IExceptionReporter
{
  private readonly IExceptionClassifier _classifier;
  private readonly ILogger<VportalExceptionReporter> _logger;
  private readonly VportalExceptionOptions _options;
  private readonly IHub? _hub;

  public VportalExceptionReporter(
      IExceptionClassifier classifier,
      ILogger<VportalExceptionReporter> logger,
      IOptions<VportalExceptionOptions> options,
      IHub? hub = null)
  {
    _classifier = classifier;
    _logger = logger;
    _options = options.Value;
    _hub = hub;
  }

  public bool ShouldSuppress(Exception ex)
  {
    return ShouldSuppressInternal(_classifier.Classify(ex), ex);
  }

  public void Report(Exception ex, IReadOnlyDictionary<string, object?>? context = null)
  {
    var kind = _classifier.Classify(ex);
    var suppress = ShouldSuppressInternal(kind, ex);

    var scopeContext = BuildScopeContext(context, kind);
    using var scope = scopeContext == null ? null : _logger.BeginScope(scopeContext);

    if (suppress)
    {
      LogSuppressed(kind, ex);
      return;
    }

    LogCaptured(kind, ex);

    if (_hub == null)
    {
      return;
    }

    _hub.CaptureException(ex, sentryScope =>
    {
      var tenant = GetContextString(context, VportalLogProperties.Tenant) ?? "Host";
      var traceId = GetContextString(context, VportalLogProperties.TraceId) ?? Activity.Current?.TraceId.ToString() ?? "Unknown";

      sentryScope.SetTag(VportalLogProperties.Tenant, tenant);
      sentryScope.SetTag(VportalLogProperties.TraceId, traceId);
      sentryScope.SetTag(VportalLogProperties.ExceptionKind, kind.ToString());

      var route = SelectRoute(context);
      if (!string.IsNullOrWhiteSpace(route))
      {
        sentryScope.SetTag(VportalLogProperties.Route, route);
      }

      if (context != null)
      {
        foreach (var entry in context)
        {
          if (IsStandardTag(entry.Key))
          {
            continue;
          }

          sentryScope.SetExtra(entry.Key, entry.Value ?? string.Empty);
        }
      }
    });
  }

  private bool ShouldSuppressInternal(ExceptionKind kind, Exception ex)
  {
    if (_options.SuppressPredicate?.Invoke(ex) == true)
    {
      return true;
    }

    return kind switch
    {
      ExceptionKind.Business => !_options.CaptureBusinessExceptionsToSentry,
      ExceptionKind.Cancellation => !_options.CaptureCancellationToSentry,
      _ => false
    };
  }

  private void LogSuppressed(ExceptionKind kind, Exception ex)
  {
    switch (kind)
    {
      case ExceptionKind.Business:
        _logger.LogInformation(ex, "Business exception suppressed");
        break;
      case ExceptionKind.Cancellation:
        _logger.LogDebug(ex, "Cancellation exception suppressed");
        break;
      default:
        _logger.LogDebug(ex, "Exception suppressed");
        break;
    }
  }

  private void LogCaptured(ExceptionKind kind, Exception ex)
  {
    switch (kind)
    {
      case ExceptionKind.Business:
        _logger.LogInformation(ex, "Business exception captured");
        break;
      case ExceptionKind.Cancellation:
        _logger.LogDebug(ex, "Cancellation exception captured");
        break;
      default:
        _logger.LogError(ex, "Unexpected exception captured");
        break;
    }
  }

  private string? SelectRoute(IReadOnlyDictionary<string, object?>? context)
  {
    var route = GetContextString(context, VportalLogProperties.Route);
    var path = GetContextString(context, VportalLogProperties.Path);

    if (_options.PreferRouteTemplateOverPath)
    {
      return !string.IsNullOrWhiteSpace(route) ? route : path;
    }

    return !string.IsNullOrWhiteSpace(path) ? path : route;
  }

  private static string? GetContextString(IReadOnlyDictionary<string, object?>? context, string key)
  {
    if (context == null)
    {
      return null;
    }

    if (!context.TryGetValue(key, out var value) || value == null)
    {
      return null;
    }

    return value.ToString();
  }

  private static bool IsStandardTag(string key)
    => key == VportalLogProperties.Tenant
       || key == VportalLogProperties.TraceId
       || key == VportalLogProperties.ExceptionKind
       || key == VportalLogProperties.Route;

  private static IReadOnlyDictionary<string, object?>? BuildScopeContext(IReadOnlyDictionary<string, object?>? context, ExceptionKind kind)
  {
    if (context == null || context.Count == 0)
    {
      return new Dictionary<string, object?>(StringComparer.Ordinal)
      {
        [VportalLogProperties.ExceptionKind] = kind.ToString()
      };
    }

    var scopeContext = new Dictionary<string, object?>(context, StringComparer.Ordinal)
    {
      [VportalLogProperties.ExceptionKind] = kind.ToString()
    };

    return scopeContext;
  }
}
