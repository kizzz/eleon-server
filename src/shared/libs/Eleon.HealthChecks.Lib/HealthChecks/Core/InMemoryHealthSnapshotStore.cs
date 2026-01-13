using EleonsoftSdk.modules.HealthCheck.Module.Core.Models;

namespace EleonsoftSdk.modules.HealthCheck.Module.Core;

/// <summary>
/// In-memory implementation of IHealthSnapshotStore.
/// Thread-safe using lock-free reads with volatile writes.
/// </summary>
public class InMemoryHealthSnapshotStore : IHealthSnapshotStore
{
    private volatile HealthSnapshot? _latest;

    public void Store(HealthSnapshot snapshot)
    {
        _latest = snapshot;
    }

    public HealthSnapshot? GetLatest()
    {
        return _latest;
    }

    public void Clear()
    {
        _latest = null;
    }
}
