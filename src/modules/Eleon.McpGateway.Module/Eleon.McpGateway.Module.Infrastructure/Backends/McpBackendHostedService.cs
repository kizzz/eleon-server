using Eleon.McpGateway.Module.Domain;
using Eleon.Logging.Lib.VportalLogging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Eleon.McpGateway.Module.Infrastructure.Backends;

internal sealed class McpBackendHostedService(
    IMcpBackendRegistry registry,
    ILogger<McpBackendHostedService> logger,
    IBoundaryLogger boundaryLogger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var _ = boundaryLogger.Begin("HostedService McpBackendHostedService Start");
        foreach (var backend in registry.GetAll())
        {
            logger.LogInformation("Starting MCP backend {Backend}.", backend.Name);
            await backend.StartAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var _ = boundaryLogger.Begin("HostedService McpBackendHostedService Stop");
        foreach (var backend in registry.GetAll())
        {
            logger.LogInformation("Stopping MCP backend {Backend}.", backend.Name);
            await backend.DisposeAsync().ConfigureAwait(false);
        }
    }
}
