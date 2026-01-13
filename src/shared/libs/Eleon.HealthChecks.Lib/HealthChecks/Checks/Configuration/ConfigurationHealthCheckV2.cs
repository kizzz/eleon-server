using EleonsoftSdk.modules.HealthCheck.Module.Implementations.CheckConfiguration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.Configuration;

/// <summary>
/// Configuration health check implementing IHealthCheck.
/// Validates configuration sections.
/// </summary>
public class ConfigurationHealthCheckV2 : IHealthCheck
{
    private readonly CheckConfigurationService _checkService;
    private readonly ILogger<ConfigurationHealthCheckV2> _logger;

    public ConfigurationHealthCheckV2(
        CheckConfigurationService checkService,
        ILogger<ConfigurationHealthCheckV2> logger)
    {
        _checkService = checkService;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _checkService.CheckAsync();

            var data = new Dictionary<string, object>
            {
                ["config.valid"] = result.Key,
                ["config.checked_count"] = result.Value?.Count ?? 0
            };

            if (result.Value != null)
            {
                var errors = result.Value.Where(r => r.Value.IsErrored).ToList();
                var valid = result.Value.Where(r => !r.Value.IsErrored).ToList();

                data["config.errors_count"] = errors.Count;
                data["config.valid_count"] = valid.Count;

                if (errors.Any())
                {
                    var errorDetails = errors.Select(e => $"{e.Key}:{string.Join(";", e.Value.ErrorMessage)}").ToList();
                    data["config.errors"] = string.Join(" | ", errorDetails);
                }
            }

            if (result.Key)
            {
                return HealthCheckResult.Healthy("Configuration is valid", data);
            }

            return HealthCheckResult.Unhealthy("Configuration validation failed", data: data);
        }
        catch (OperationCanceledException)
        {
            return HealthCheckResult.Unhealthy("Configuration check cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in configuration health check");
            return HealthCheckResult.Unhealthy($"Error: {ex.Message}");
        }
    }
}
