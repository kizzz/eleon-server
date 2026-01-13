namespace EleonsoftSdk.modules.HealthCheck.Module.Delivery;

/// <summary>
/// Options for health check publishing.
/// </summary>
public class HealthPublishingOptions
{
    /// <summary>
    /// Publish when any check fails.
    /// </summary>
    public bool PublishOnFailure { get; set; } = true;

    /// <summary>
    /// Publish when status changes.
    /// </summary>
    public bool PublishOnChange { get; set; } = false;

    /// <summary>
    /// Publish periodically (heartbeat).
    /// </summary>
    public int PublishIntervalMinutes { get; set; } = 5;

    /// <summary>
    /// Throttle: maximum publishes per minute.
    /// </summary>
    public int MaxPublishesPerMinute { get; set; } = 1;
}
