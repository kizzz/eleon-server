using EleonsoftSdk.modules.HealthCheck.Module.Core.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;

namespace EleonsoftSdk.modules.HealthCheck.Module.Core;

/// <summary>
/// Coordinates health check runs with thread-safe single-run enforcement
/// and proper timeout/cancellation handling.
/// </summary>
public class HealthRunCoordinator : IHealthRunCoordinator
{
    private readonly HealthCheckService _healthCheckService;
    private readonly IHealthSnapshotStore _snapshotStore;
    private readonly IHealthReportBuilder _reportBuilder;
    private readonly ILogger<HealthRunCoordinator> _logger;
    private readonly SemaphoreSlim _runLock;
    private readonly string _applicationName;

    private volatile bool _isRunning;
    private HealthSnapshot? _latestSnapshot;

    public bool IsRunning => _isRunning;

    public HealthRunCoordinator(
        HealthCheckService healthCheckService,
        IHealthSnapshotStore snapshotStore,
        IHealthReportBuilder reportBuilder,
        ILogger<HealthRunCoordinator> logger,
        string applicationName)
    {
        _healthCheckService = healthCheckService;
        _snapshotStore = snapshotStore;
        _reportBuilder = reportBuilder;
        _logger = logger;
        _applicationName = applicationName;
        _runLock = new SemaphoreSlim(1, 1);
    }

    public async Task<HealthSnapshot?> RunAsync(
        string type,
        string initiatorName,
        HealthRunOptions? options = null,
        CancellationToken ct = default)
    {
        // Try to acquire lock (non-blocking)
        if (!await _runLock.WaitAsync(0, ct))
        {
            _logger.LogDebug("Health check run already in progress, skipping");
            return null;
        }

        try
        {
            _isRunning = true;
            var sw = Stopwatch.StartNew();
            var healthCheckId = Guid.NewGuid();
            var createdAt = DateTime.UtcNow;

            _logger.LogInformation("Starting health check run {HealthCheckId} of type {Type} initiated by {Initiator}", 
                healthCheckId, type, initiatorName);

            // Create linked cancellation token with timeout
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var timeoutSeconds = options?.CheckTimeoutSeconds ?? 30;
            cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

            // Determine tags filter
            var tags = options?.Tags;
            Func<HealthCheckRegistration, bool> predicate = tags != null && tags.Length > 0
                ? reg => reg.Tags.Any(t => tags.Contains(t, StringComparer.OrdinalIgnoreCase))
                : _ => true;

            // Run health checks
            HealthReport healthReport;
            try
            {
                healthReport = await _healthCheckService.CheckHealthAsync(predicate, cts.Token);
            }
            catch (OperationCanceledException) when (cts.Token.IsCancellationRequested && !ct.IsCancellationRequested)
            {
                _logger.LogWarning("Health check run {HealthCheckId} timed out after {TimeoutSeconds} seconds", 
                    healthCheckId, timeoutSeconds);
                throw new TimeoutException($"Health check run timed out after {timeoutSeconds} seconds");
            }

            sw.Stop();

            // Build ETO from report
            var healthCheckEto = _reportBuilder.BuildHealthCheckEto(
                healthReport,
                healthCheckId,
                type,
                initiatorName,
                createdAt);

            // Create snapshot
            var snapshot = new HealthSnapshot(
                id: healthCheckId,
                createdAt: createdAt,
                type: type,
                initiatorName: initiatorName,
                healthCheck: healthCheckEto,
                isComplete: true,
                duration: sw.Elapsed);

            // Store snapshot
            _snapshotStore.Store(snapshot);
            _latestSnapshot = snapshot;

            _logger.LogInformation("Completed health check run {HealthCheckId} in {DurationMs}ms with status {Status}",
                healthCheckId, sw.ElapsedMilliseconds, healthCheckEto.Status);

            return snapshot;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during health check run");
            throw;
        }
        finally
        {
            _isRunning = false;
            _runLock.Release();
        }
    }

    public HealthSnapshot? GetLatestSnapshot()
    {
        return _snapshotStore.GetLatest() ?? _latestSnapshot;
    }
}
