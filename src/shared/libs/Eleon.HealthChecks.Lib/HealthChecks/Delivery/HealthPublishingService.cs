using EleonsoftSdk.modules.HealthCheck.Module.Core;
using EleonsoftSdk.modules.HealthCheck.Module.Core.Models;
using EleonsoftSdk.modules.HealthCheck.Module.Delivery;
using Eleon.Logging.Lib.VportalLogging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace EleonsoftSdk.modules.HealthCheck.Module.Delivery;

/// <summary>
/// Background service that publishes health check snapshots based on policies.
/// </summary>
public class HealthPublishingService : BackgroundService
{
    private readonly IHealthRunCoordinator _coordinator;
    private readonly IHealthPublisher[] _publishers;
    private readonly HealthPublishingOptions _options;
    private readonly ILogger<HealthPublishingService> _logger;
    private readonly IBoundaryLogger _boundaryLogger;

    private HealthSnapshot? _lastPublishedSnapshot;
    private readonly ConcurrentQueue<DateTime> _publishTimestamps = new();
    private readonly object _publishLock = new();

    public HealthPublishingService(
        IHealthRunCoordinator coordinator,
        IEnumerable<IHealthPublisher> publishers,
        IOptions<HealthPublishingOptions> options,
        ILogger<HealthPublishingService> logger,
        IBoundaryLogger boundaryLogger)
    {
        _coordinator = coordinator;
        _publishers = publishers.ToArray();
        _options = options.Value;
        _logger = logger;
        _boundaryLogger = boundaryLogger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var _ = _boundaryLogger.Begin("HostedService HealthPublishingService");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var latest = _coordinator.GetLatestSnapshot();
                if (latest != null)
                {
                    await TryPublishAsync(latest, stoppingToken);
                }

                await Task.Delay(TimeSpan.FromMinutes(_options.PublishIntervalMinutes), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected on shutdown
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in health publishing service");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Retry after delay
            }
        }
    }

    private async Task TryPublishAsync(HealthSnapshot snapshot, CancellationToken ct)
    {
        // Throttle check
        if (!CanPublish())
        {
            _logger.LogDebug("Publishing throttled, skipping snapshot {SnapshotId}", snapshot.Id);
            return;
        }

        // Policy checks
        bool shouldPublish = false;

        if (_options.PublishOnFailure)
        {
            var hasFailures = snapshot.HealthCheck.Reports?.Any(r => r.Status == EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants.HealthCheckStatus.Failed) ?? false;
            if (hasFailures)
            {
                shouldPublish = true;
                _logger.LogDebug("Publishing due to failure in snapshot {SnapshotId}", snapshot.Id);
            }
        }

        if (_options.PublishOnChange)
        {
            if (_lastPublishedSnapshot == null || HasStatusChanged(_lastPublishedSnapshot, snapshot))
            {
                shouldPublish = true;
                _logger.LogDebug("Publishing due to status change in snapshot {SnapshotId}", snapshot.Id);
            }
        }

        // Periodic heartbeat (always publish if enough time has passed)
        if (!shouldPublish && _lastPublishedSnapshot == null)
        {
            shouldPublish = true;
            _logger.LogDebug("Publishing initial snapshot {SnapshotId}", snapshot.Id);
        }

        if (!shouldPublish)
        {
            return;
        }

        // Publish to all publishers
        var tasks = _publishers.Select(p => PublishToPublisherAsync(p, snapshot, ct));
        await Task.WhenAll(tasks);

        _lastPublishedSnapshot = snapshot;
        RecordPublishTimestamp();
    }

    private async Task PublishToPublisherAsync(IHealthPublisher publisher, HealthSnapshot snapshot, CancellationToken ct)
    {
        try
        {
            var success = await publisher.PublishAsync(snapshot, ct);
            if (success)
            {
                _logger.LogDebug("Published snapshot {SnapshotId} to {Publisher}", snapshot.Id, publisher.Name);
            }
            else
            {
                _logger.LogWarning("Failed to publish snapshot {SnapshotId} to {Publisher}", snapshot.Id, publisher.Name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing snapshot {SnapshotId} to {Publisher}", snapshot.Id, publisher.Name);
        }
    }

    private bool CanPublish()
    {
        lock (_publishLock)
        {
            var now = DateTime.UtcNow;
            var cutoff = now.AddMinutes(-1);

            // Remove old timestamps
            while (_publishTimestamps.TryPeek(out var timestamp) && timestamp < cutoff)
            {
                _publishTimestamps.TryDequeue(out _);
            }

            // Check if we're under the limit
            return _publishTimestamps.Count < _options.MaxPublishesPerMinute;
        }
    }

    private void RecordPublishTimestamp()
    {
        lock (_publishLock)
        {
            _publishTimestamps.Enqueue(DateTime.UtcNow);
        }
    }

    private static bool HasStatusChanged(HealthSnapshot old, HealthSnapshot @new)
    {
        var oldStatus = old.HealthCheck.Status;
        var newStatus = @new.HealthCheck.Status;
        return oldStatus != newStatus;
    }
}
