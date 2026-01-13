using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Eleon.Logging.Lib.VportalLogging;

public sealed class BoundaryLogger : IBoundaryLogger
{
  private readonly IOperationScopeFactory _operationScopeFactory;
  private readonly ILogger<BoundaryLogger> _logger;

  public BoundaryLogger(IOperationScopeFactory operationScopeFactory, ILogger<BoundaryLogger> logger)
  {
    _operationScopeFactory = operationScopeFactory;
    _logger = logger;
  }

  public IBoundaryScope Begin(string boundaryName, IReadOnlyDictionary<string, object?>? context = null)
  {
    var scopeContext = BuildContext(boundaryName, context, tenantId: null);
    var scope = _operationScopeFactory.Begin(boundaryName, scopeContext);
    return new BoundaryScope(_logger, scope, boundaryName, scopeContext);
  }

  public IBoundaryScope BeginJob(string jobName, string? jobId = null, string? tenantId = null, string? correlationId = null)
  {
    var context = new Dictionary<string, object?>(StringComparer.Ordinal)
    {
      [VportalLogProperties.Component] = "Job",
      [VportalLogProperties.JobName] = jobName
    };

    if (!string.IsNullOrWhiteSpace(jobId))
    {
      context[VportalLogProperties.JobId] = jobId;
    }

    if (!string.IsNullOrWhiteSpace(correlationId))
    {
      context[VportalLogProperties.CorrelationId] = correlationId;
    }

    return Begin($"Job {jobName}", context, tenantId);
  }

  public IBoundaryScope BeginConsumer(string messageType, string? messageId = null, string? correlationId = null, string? tenantId = null)
  {
    var context = new Dictionary<string, object?>(StringComparer.Ordinal)
    {
      [VportalLogProperties.Component] = "Consumer",
      [VportalLogProperties.MessageType] = messageType
    };

    if (!string.IsNullOrWhiteSpace(messageId))
    {
      context[VportalLogProperties.MessageId] = messageId;
    }

    if (!string.IsNullOrWhiteSpace(correlationId))
    {
      context[VportalLogProperties.CorrelationId] = correlationId;
    }

    return Begin($"Consume {messageType}", context, tenantId);
  }

  private IBoundaryScope Begin(string boundaryName, IReadOnlyDictionary<string, object?>? context, string? tenantId)
  {
    var scopeContext = BuildContext(boundaryName, context, tenantId);
    var scope = _operationScopeFactory.Begin(boundaryName, scopeContext);
    return new BoundaryScope(_logger, scope, boundaryName, scopeContext);
  }

  private static IReadOnlyDictionary<string, object?> BuildContext(
      string boundaryName,
      IReadOnlyDictionary<string, object?>? context,
      string? tenantId)
  {
    var scope = new Dictionary<string, object?>(StringComparer.Ordinal)
    {
      [VportalLogProperties.Operation] = boundaryName
    };

    var tenantValue = string.IsNullOrWhiteSpace(tenantId) ? "Host" : tenantId;
    scope[VportalLogProperties.Tenant] = tenantValue;

    if (context != null)
    {
      foreach (var entry in context)
      {
        if (!scope.ContainsKey(entry.Key))
        {
          scope[entry.Key] = entry.Value;
        }
      }
    }

    var activity = Activity.Current;
    if (activity != null)
    {
      scope[VportalLogProperties.TraceId] = activity.TraceId.ToString();
      scope[VportalLogProperties.SpanId] = activity.SpanId.ToString();
    }

    return scope;
  }

  private sealed class BoundaryScope : IBoundaryScope
  {
    private readonly ILogger _logger;
    private readonly IDisposable _scope;
    private readonly string _boundaryName;
    private readonly Stopwatch _stopwatch;
    private bool _disposed;

    public BoundaryScope(
        ILogger logger,
        IDisposable scope,
        string boundaryName,
        IReadOnlyDictionary<string, object?> context)
    {
      _logger = logger;
      _scope = scope;
      _boundaryName = boundaryName;
      Context = context;
      _stopwatch = Stopwatch.StartNew();
      _logger.LogDebug("{Boundary} started", _boundaryName);
    }

    public IReadOnlyDictionary<string, object?> Context { get; }

    public void Dispose()
    {
      if (_disposed)
      {
        return;
      }

      _disposed = true;
      _stopwatch.Stop();
      _logger.LogDebug("{Boundary} finished in {ElapsedMs}ms", _boundaryName, _stopwatch.Elapsed.TotalMilliseconds);
      _scope.Dispose();
    }
  }
}
