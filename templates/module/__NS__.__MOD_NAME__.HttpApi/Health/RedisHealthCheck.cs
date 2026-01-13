using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace __NS__.__MOD_NAME__.HttpApi.Health;

/// <summary>Redis health via TCP probe (no extra packages). Supply "host:port".</summary>
public sealed class RedisHealthCheck : IHealthCheck
{
    private readonly string _endpoint;
    public RedisHealthCheck(string endpoint) => _endpoint = endpoint;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var parts = _endpoint.Split(':', StringSplitOptions.RemoveEmptyEntries);
            var host = parts[0];
            var port = parts.Length > 1 && int.TryParse(parts[1], out var p) ? p : 6379;
            var ok = await TcpProbe.TryConnectAsync(host, port, TimeSpan.FromSeconds(2), cancellationToken);
            return ok ? HealthCheckResult.Healthy($"Redis tcp OK {host}:{port}") : HealthCheckResult.Unhealthy($"Redis tcp FAIL {host}:{port}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Redis parse/connect error for '{_endpoint}'", ex);
        }
    }
}
