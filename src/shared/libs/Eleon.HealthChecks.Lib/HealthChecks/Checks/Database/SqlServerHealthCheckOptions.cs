namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.Database;

/// <summary>
/// Options for SQL Server health checks.
/// </summary>
public class SqlServerHealthCheckOptions
{
    /// <summary>
    /// Enable diagnostics checks (expensive operations, requires diag tag).
    /// </summary>
    public bool EnableDiagnostics { get; set; } = false;

    /// <summary>
    /// TTL cache duration for diagnostics in minutes.
    /// </summary>
    public int DiagnosticsCacheMinutes { get; set; } = 10;

    /// <summary>
    /// Maximum number of tables to return in diagnostics.
    /// </summary>
    public int MaxTablesInDiagnostics { get; set; } = 100;
}
