using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace __NS__.__MOD_NAME__.HttpApi.Health;

/// <summary>RabbitMQ health via TCP probe (no extra packages). Supply "host:port".</summary>
public sealed class RabbitMqHealthCheck : IHealthCheck
{
    private readonly string _endpoint;
    public RabbitMqHealthCheck(string endpoint) => _endpoint = endpoint;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var parts = _endpoint.Split(':', StringSplitOptions.RemoveEmptyEntries);
            var host = parts[0];
            var port = parts.Length > 1 && int.TryParse(parts[1], out var p) ? p : 5672;
            var ok = await TcpProbe.TryConnectAsync(host, port, TimeSpan.FromSeconds(2), cancellationToken);
            return ok ? HealthCheckResult.Healthy($"RabbitMQ tcp OK {host}:{port}") : HealthCheckResult.Unhealthy($"RabbitMQ tcp FAIL {host}:{port}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Rabbit parse/connect error for '{_endpoint}'", ex);
        }
    }
}
