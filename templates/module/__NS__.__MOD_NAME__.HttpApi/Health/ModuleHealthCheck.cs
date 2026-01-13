using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace __NS__.__MOD_NAME__.HttpApi.Health;

/// <summary>Lightweight module self-check (fast; no I/O).</summary>
public sealed class ModuleHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // Add lightweight checks if needed (e.g., static flags, in-memory queues)
        return Task.FromResult(HealthCheckResult.Healthy("Module baseline OK"));
    }
}
