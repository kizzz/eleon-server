using System;
using System.Collections.Generic;

namespace Eleon.Logging.Lib.VportalLogging;

public interface IBoundaryScope : IDisposable
{
  IReadOnlyDictionary<string, object?> Context { get; }
}

public interface IBoundaryLogger
{
  IBoundaryScope Begin(string boundaryName, IReadOnlyDictionary<string, object?>? context = null);

  IBoundaryScope BeginJob(string jobName, string? jobId = null, string? tenantId = null, string? correlationId = null);

  IBoundaryScope BeginConsumer(string messageType, string? messageId = null, string? correlationId = null, string? tenantId = null);
}
