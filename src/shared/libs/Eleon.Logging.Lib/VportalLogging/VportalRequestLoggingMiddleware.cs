using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using Volo.Abp.MultiTenancy;

namespace Eleon.Logging.Lib.VportalLogging;

public sealed class VportalRequestLoggingMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<VportalRequestLoggingMiddleware> _logger;
  private readonly IOperationScopeFactory _operationScopeFactory;
  private readonly IExceptionReporter _exceptionReporter;
  private readonly VportalRequestLoggingOptions _options;

  public VportalRequestLoggingMiddleware(
      RequestDelegate next,
      ILogger<VportalRequestLoggingMiddleware> logger,
      IOperationScopeFactory operationScopeFactory,
      IExceptionReporter exceptionReporter,
      IOptions<VportalRequestLoggingOptions> options)
  {
    _next = next;
    _logger = logger;
    _operationScopeFactory = operationScopeFactory;
    _exceptionReporter = exceptionReporter;
    _options = options.Value;
  }

  public async Task InvokeAsync(HttpContext context, ICurrentTenant? currentTenant = null)
  {
    if (!_options.Enable || IsExcludedPath(context.Request.Path))
    {
      await _next(context);
      return;
    }

    var activity = Activity.Current;
    var traceId = activity?.TraceId.ToString() ?? context.TraceIdentifier;
    var spanId = activity?.SpanId.ToString();

    var tenantId = currentTenant?.Id?.ToString();
    var tenantValue = string.IsNullOrWhiteSpace(tenantId) ? "Host" : tenantId;

    var method = context.Request.Method;
    var endpoint = context.GetEndpoint();
    var route = (endpoint as RouteEndpoint)?.RoutePattern?.RawText;
    var path = context.Request.Path.HasValue ? context.Request.Path.Value : null;

    var routeOrPath = route ?? path ?? "unknown";
    var operationName = $"{method} {routeOrPath}";

    var scopeContext = new Dictionary<string, object?>(StringComparer.Ordinal)
    {
      [VportalLogProperties.Tenant] = tenantValue,
      [VportalLogProperties.TraceId] = traceId,
      [VportalLogProperties.SpanId] = spanId,
      [VportalLogProperties.HttpMethod] = method,
      [VportalLogProperties.Component] = nameof(VportalRequestLoggingMiddleware)
    };

    if (!string.IsNullOrWhiteSpace(route))
    {
      scopeContext[VportalLogProperties.Route] = route;
    }

    if (!string.IsNullOrWhiteSpace(path) && (string.IsNullOrWhiteSpace(route) || !_options.PreferRouteTemplateOverPath))
    {
      scopeContext[VportalLogProperties.Path] = path;
    }

    using var scope = _operationScopeFactory.Begin(operationName, scopeContext);

    _logger.LogDebug("HTTP request started");

    var stopwatch = Stopwatch.StartNew();

    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      _exceptionReporter.Report(ex, scopeContext);
      throw;
    }
    finally
    {
      stopwatch.Stop();
      _logger.LogDebug(
          "HTTP request finished with {StatusCode} in {ElapsedMs}ms",
          context.Response.StatusCode,
          stopwatch.Elapsed.TotalMilliseconds);
    }
  }

  private bool IsExcludedPath(PathString path)
  {
    if (!path.HasValue)
    {
      return false;
    }

    if (_options.ExcludedPathPrefixes == null || _options.ExcludedPathPrefixes.Count == 0)
    {
      return false;
    }

    var pathValue = path.Value ?? string.Empty;

    foreach (var prefix in _options.ExcludedPathPrefixes)
    {
      if (string.IsNullOrWhiteSpace(prefix))
      {
        continue;
      }

      if (pathValue.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }
    }

    return false;
  }
}
