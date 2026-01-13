namespace EleonsoftSdk.modules.HealthCheck.Module.Core.Models;

/// <summary>
/// Configuration options for health check runs.
/// </summary>
public class HealthRunOptions
{
    /// <summary>
    /// Timeout for individual checks in seconds.
    /// </summary>
    public int CheckTimeoutSeconds { get; set; } = 5;

    /// <summary>
    /// Enable diagnostics checks (expensive operations).
    /// </summary>
    public bool EnableDiagnostics { get; set; } = false;

    /// <summary>
    /// Tags to include in the run (live, ready, diag).
    /// </summary>
    public string[]? Tags { get; set; }

    /// <summary>
    /// Maximum number of concurrent checks.
    /// </summary>
    public int MaxConcurrency { get; set; } = 10;
}
