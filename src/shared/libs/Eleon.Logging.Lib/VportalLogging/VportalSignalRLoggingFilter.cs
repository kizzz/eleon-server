using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using Volo.Abp.MultiTenancy;

namespace Eleon.Logging.Lib.VportalLogging;

public sealed class VportalSignalRLoggingFilter : IHubFilter
{
  private readonly IOperationScopeFactory _operationScopeFactory;
  private readonly IExceptionReporter _exceptionReporter;
  private readonly ILogger<VportalSignalRLoggingFilter> _logger;
  private readonly ICurrentTenant? _currentTenant;
  private readonly VportalLoggingHostOptions _hostOptions;

  public VportalSignalRLoggingFilter(
      IOperationScopeFactory operationScopeFactory,
      IExceptionReporter exceptionReporter,
      ILogger<VportalSignalRLoggingFilter> logger,
      IOptions<VportalLoggingHostOptions> hostOptions,
      ICurrentTenant? currentTenant = null)
  {
    _operationScopeFactory = operationScopeFactory;
    _exceptionReporter = exceptionReporter;
    _logger = logger;
    _currentTenant = currentTenant;
    _hostOptions = hostOptions.Value;
  }

  public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext context, Func<HubInvocationContext, ValueTask<object?>> next)
  {
    if (!_hostOptions.EnableSignalRLogging)
    {
      return await next(context);
    }

    var hubName = context.Hub?.GetType().Name ?? "Hub";
    var operationName = $"SignalR {hubName}.{context.HubMethodName}";
    var scopeContext = BuildContext(hubName, context.HubMethodName);

    using var scope = _operationScopeFactory.Begin(operationName, scopeContext);

    _logger.LogDebug("SignalR hub method started");

    try
    {
      var result = await next(context);
      return result;
    }
    catch (Exception ex)
    {
      _exceptionReporter.Report(ex, scopeContext);
      throw;
    }
    finally
    {
      _logger.LogDebug("SignalR hub method finished");
    }
  }

  public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
  {
    if (!_hostOptions.EnableSignalRLogging)
    {
      await next(context);
      return;
    }

    var hubName = context.Hub?.GetType().Name ?? "Hub";
    var operationName = $"SignalR {hubName}.Connected";
    var scopeContext = BuildContext(hubName, "Connected");

    using var scope = _operationScopeFactory.Begin(operationName, scopeContext);

    _logger.LogDebug("SignalR connected");

    try
    {
      await next(context);
    }
    catch (Exception ex)
    {
      _exceptionReporter.Report(ex, scopeContext);
      throw;
    }
  }

  public async Task OnDisconnectedAsync(HubLifetimeContext context, Exception? exception, Func<HubLifetimeContext, Exception?, Task> next)
  {
    if (!_hostOptions.EnableSignalRLogging)
    {
      await next(context, exception);
      return;
    }

    var hubName = context.Hub?.GetType().Name ?? "Hub";
    var operationName = $"SignalR {hubName}.Disconnected";
    var scopeContext = BuildContext(hubName, "Disconnected");

    using var scope = _operationScopeFactory.Begin(operationName, scopeContext);

    if (exception != null)
    {
      _exceptionReporter.Report(exception, scopeContext);
    }

    await next(context, exception);
  }

  private IReadOnlyDictionary<string, object?> BuildContext(string hubName, string operation)
  {
    var tenantId = _currentTenant?.Id?.ToString();
    var tenantValue = string.IsNullOrWhiteSpace(tenantId) ? "Host" : tenantId;

    var context = new Dictionary<string, object?>(StringComparer.Ordinal)
    {
      [VportalLogProperties.Tenant] = tenantValue,
      [VportalLogProperties.Component] = hubName,
      [VportalLogProperties.Operation] = operation
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
