using System;
using System.Collections.Generic;
using System.Reflection;

namespace Eleon.Logging.Lib.VportalLogging;

public static class BoundaryLoggingExtensions
{
  public static IBoundaryScope BeginConsumer(this IBoundaryLogger boundaryLogger, object? message)
  {
    if (message == null)
    {
      return boundaryLogger.BeginConsumer("Unknown");
    }

    var job = TryGetPropertyValue(message, "BackgroundJob");
    if (job != null)
    {
      return boundaryLogger.BeginJobFromMessage(message, job);
    }

    var messageType = message.GetType().Name;
    var messageId = TryGetPropertyString(message, "MessageId") ?? TryGetPropertyString(message, "Id");
    var correlationId = TryGetPropertyString(message, "CorrelationId") ?? TryGetPropertyString(message, "ExecutionId");
    var tenantId = TryGetPropertyString(message, "TenantId");

    return boundaryLogger.BeginConsumer(messageType, messageId, correlationId, tenantId);
  }

  private static IBoundaryScope BeginJobFromMessage(this IBoundaryLogger boundaryLogger, object message, object job)
  {
    var jobName = TryGetPropertyString(job, "Type") ?? job.GetType().Name;
    var jobId = TryGetPropertyString(job, "Id");
    var tenantId = TryGetPropertyString(message, "TenantId") ?? TryGetPropertyString(job, "TenantId");
    var correlationId = TryGetPropertyString(message, "ExecutionId") ?? TryGetPropertyString(message, "CorrelationId");

    return boundaryLogger.BeginJob(jobName, jobId, tenantId, correlationId);
  }

  private static object? TryGetPropertyValue(object instance, string propertyName)
  {
    var property = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
    return property?.GetValue(instance);
  }

  private static string? TryGetPropertyString(object instance, string propertyName)
  {
    var value = TryGetPropertyValue(instance, propertyName);
    return value?.ToString();
  }
}
