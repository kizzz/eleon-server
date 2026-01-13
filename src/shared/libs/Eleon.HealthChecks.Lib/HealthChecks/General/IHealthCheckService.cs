using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;

namespace EleonsoftSdk.modules.HealthCheck.Module.General;

public interface IHealthCheckService
{
  public Task<bool> AddBulkReportsAsync(List<HealthCheckReportEto> reports, CancellationToken cancellationToken = default);
  public Task<bool> AddReportAsync(HealthCheckReportEto report, CancellationToken cancellationToken = default);
  public Task<HealthCheckResponse> StartHealthCheckAsync(string type, string initiatorName, CancellationToken cancellationToken = default);
  public Task<HealthCheckResponse> SendHealthCheckAsync(HealthCheckEto healthCheck, CancellationToken cancellationToken = default);
}


public record HealthCheckResponse(bool Success, Guid HealthCheckId, string Error);
