using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;

namespace EleonsoftSdk.modules.HealthCheck.Module.Core.Models;

/// <summary>
/// Immutable snapshot of health state at a point in time.
/// Thread-safe for concurrent reads.
/// </summary>
public sealed class HealthSnapshot
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Type { get; init; } = string.Empty;
    public string InitiatorName { get; init; } = string.Empty;
    public HealthCheckEto HealthCheck { get; init; } = null!;
    public bool IsComplete { get; init; }
    public TimeSpan Duration { get; init; }

    public HealthSnapshot(
        Guid id,
        DateTime createdAt,
        string type,
        string initiatorName,
        HealthCheckEto healthCheck,
        bool isComplete,
        TimeSpan duration)
    {
        Id = id;
        CreatedAt = createdAt;
        Type = type;
        InitiatorName = initiatorName;
        HealthCheck = healthCheck;
        IsComplete = isComplete;
        Duration = duration;
    }
}
