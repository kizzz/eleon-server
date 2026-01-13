using EleonsoftSdk.modules.HealthCheck.Module.Core.Models;

namespace EleonsoftSdk.modules.HealthCheck.Module.Core;

/// <summary>
/// Stores and retrieves health check snapshots.
/// Default implementation is in-memory; can be extended to Redis/Postgres.
/// </summary>
public interface IHealthSnapshotStore
{
    /// <summary>
    /// Stores a snapshot, replacing any previous snapshot.
    /// </summary>
    void Store(HealthSnapshot snapshot);

    /// <summary>
    /// Gets the latest snapshot, or null if none exists.
    /// </summary>
    HealthSnapshot? GetLatest();

    /// <summary>
    /// Clears all stored snapshots.
    /// </summary>
    void Clear();
}
