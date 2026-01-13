using Eleon.McpGateway.Module.Domain;
using Eleon.Logging.Lib.VportalLogging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eleon.McpGateway.Module.Infrastructure.Sessions;

public sealed class McpSessionCleanupService : IHostedService
{
    private readonly IMcpSessionRegistry registry;
    private readonly McpSessionOptions options;
    private readonly ILogger<McpSessionCleanupService> logger;
    private readonly IBoundaryLogger boundaryLogger;
    private IBoundaryScope? boundaryScope;
    private PeriodicTimer? timer;
    private Task? cleanupTask;
    private CancellationTokenSource? cancellationTokenSource;

    public McpSessionCleanupService(
        IMcpSessionRegistry registry,
        IOptions<McpSessionOptions> options,
        ILogger<McpSessionCleanupService> logger,
        IBoundaryLogger boundaryLogger)
    {
        this.registry = registry;
        this.options = options.Value;
        this.logger = logger;
        this.boundaryLogger = boundaryLogger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        boundaryScope = boundaryLogger.Begin("HostedService McpSessionCleanupService");
        cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timer = new PeriodicTimer(options.CleanupInterval);
        cleanupTask = ExecuteAsync(cancellationTokenSource.Token);
        logger.LogInformation("MCP session cleanup service started with interval {Interval}", options.CleanupInterval);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping MCP session cleanup service");
        timer?.Dispose();
        cancellationTokenSource?.Cancel();

        if (cleanupTask is not null)
        {
            try
            {
                await cleanupTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
        }

        cancellationTokenSource?.Dispose();
        boundaryScope?.Dispose();
        boundaryScope = null;
    }

    private async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (timer is null)
        {
            return;
        }

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                await CleanupExpiredSessionsAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when stopping
        }
    }

    private async Task CleanupExpiredSessionsAsync(CancellationToken cancellationToken)
    {
        if (registry is not McpSessionRegistry concreteRegistry)
        {
            logger.LogWarning("Registry is not McpSessionRegistry, cannot perform cleanup");
            return;
        }

        var now = DateTime.UtcNow;
        var expiredSessions = new List<string>();

        foreach (var state in concreteRegistry.GetAllSessions())
        {
            await state.Lock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var age = now - state.LastAccessedAt;
                if (age >= options.SessionTtl)
                {
                    expiredSessions.Add(state.SessionId);
                }
            }
            finally
            {
                state.Lock.Release();
            }
        }

        if (expiredSessions.Count > 0)
        {
            logger.LogInformation("Cleaning up {Count} expired sessions", expiredSessions.Count);
            foreach (var sessionId in expiredSessions)
            {
                try
                {
                    await registry.TerminateAsync(sessionId, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error terminating expired session {SessionId}", sessionId);
                }
            }
        }
    }
}
