using Eleon.McpCodexGateway.Module.Infrastructure.Services;
using Eleon.Logging.Lib.VportalLogging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Eleon.McpCodexGateway.Module.Infrastructure.Services;

internal sealed class CodexProxyHostedService(
    CodexProcessProxy proxy,
    IHostApplicationLifetime lifetime,
    ILogger<CodexProxyHostedService> logger,
    IBoundaryLogger boundaryLogger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var _ = boundaryLogger.Begin("HostedService CodexProxyHostedService");
        try
        {
            var exitCode = await proxy.RunAsync(stoppingToken).ConfigureAwait(false);
            Environment.ExitCode = exitCode;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected failure while running the Codex proxy.");
            Environment.ExitCode = -1;
        }
        finally
        {
            lifetime.StopApplication();
        }
    }
}
