using EleonsoftSdk.modules.HealthCheck.Module.Core.Models;

namespace EleonsoftSdk.modules.HealthCheck.Module.Core;

/// <summary>
/// Coordinates health check runs, enforcing single-run-at-a-time semantics
/// and proper timeout/cancellation handling.
/// </summary>
public interface IHealthRunCoordinator
{
    /// <summary>
    /// Runs health checks with the specified options.
    /// Returns null if a run is already in progress.
    /// </summary>
    Task<HealthSnapshot?> RunAsync(
        string type,
        string initiatorName,
        HealthRunOptions? options = null,
        CancellationToken ct = default);

    /// <summary>
    /// Gets the latest completed snapshot, or null if none exists.
    /// </summary>
    HealthSnapshot? GetLatestSnapshot();

    /// <summary>
    /// Gets whether a run is currently in progress.
    /// </summary>
    bool IsRunning { get; }
}
