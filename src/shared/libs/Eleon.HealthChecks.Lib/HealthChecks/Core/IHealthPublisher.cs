using EleonsoftSdk.modules.HealthCheck.Module.Core.Models;

namespace EleonsoftSdk.modules.HealthCheck.Module.Core;

/// <summary>
/// Publishes health check snapshots to external systems (HTTP, EventBus, etc.).
/// </summary>
public interface IHealthPublisher
{
    /// <summary>
    /// Publishes a snapshot.
    /// </summary>
    Task<bool> PublishAsync(HealthSnapshot snapshot, CancellationToken ct = default);

    /// <summary>
    /// Gets the publisher name for logging/identification.
    /// </summary>
    string Name { get; }
}
