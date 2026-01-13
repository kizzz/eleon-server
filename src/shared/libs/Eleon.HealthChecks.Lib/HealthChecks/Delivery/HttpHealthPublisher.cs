using EleonsoftSdk.modules.HealthCheck.Module.Core;
using EleonsoftSdk.modules.HealthCheck.Module.Core.Models;
using EleonsoftSdk.modules.HealthCheck.Module.General;
using Microsoft.Extensions.Logging;

namespace EleonsoftSdk.modules.HealthCheck.Module.Delivery;

/// <summary>
/// HTTP publisher that wraps IHealthCheckService for backward compatibility.
/// </summary>
public class HttpHealthPublisher : IHealthPublisher
{
    private readonly IHealthCheckService _healthCheckService;
    private readonly ILogger<HttpHealthPublisher> _logger;

    public string Name => "Http";

    public HttpHealthPublisher(
        IHealthCheckService healthCheckService,
        ILogger<HttpHealthPublisher> logger)
    {
        _healthCheckService = healthCheckService;
        _logger = logger;
    }

    public async Task<bool> PublishAsync(HealthSnapshot snapshot, CancellationToken ct = default)
    {
        try
        {
            var response = await _healthCheckService.SendHealthCheckAsync(snapshot.HealthCheck, ct);
            
            if (response.Success)
            {
                _logger.LogDebug("Health check snapshot {SnapshotId} published successfully", snapshot.Id);
                return true;
            }

            _logger.LogWarning("Failed to publish health check snapshot {SnapshotId}: {Error}", 
                snapshot.Id, response.Error);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing health check snapshot {SnapshotId}", snapshot.Id);
            return false;
        }
    }
}
