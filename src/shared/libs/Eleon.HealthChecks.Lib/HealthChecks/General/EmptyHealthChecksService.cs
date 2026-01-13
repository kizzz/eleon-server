using EleonsoftSdk.modules.HealthCheck.Module.General;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.HealthChecks.Lib.HealthChecks.General;
public class EmptyHealthChecksService : IHealthCheckService
{
  public Task<bool> AddBulkReportsAsync(List<HealthCheckReportEto> reports, CancellationToken cancellationToken = default)
  {
    return Task.FromResult(true);
  }

  public Task<bool> AddReportAsync(HealthCheckReportEto report, CancellationToken cancellationToken = default)
  {
    return Task.FromResult(true);
  }

  public Task<HealthCheckResponse> SendHealthCheckAsync(HealthCheckEto healthCheck, CancellationToken cancellationToken = default)
  {
    return Task.FromResult(new HealthCheckResponse(true, healthCheck.Id == Guid.Empty ? Guid.NewGuid() : healthCheck.Id, string.Empty));
  }

  public Task<HealthCheckResponse> StartHealthCheckAsync(string type, string initiatorName, CancellationToken cancellationToken = default)
  {
    return Task.FromResult(new HealthCheckResponse(true, Guid.NewGuid(), string.Empty));
  }
}
