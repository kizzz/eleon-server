namespace EleonsoftSdk.modules.HealthCheck.Module.Core.Models;

/// <summary>
/// Structured observation from a health check, providing semantic information
/// for trending, baselines, SLOs, alert routing, and consistent UI rendering.
/// </summary>
public sealed record HealthObservation(
    string Key,
    string Type,        // metric|text|json|link
    string Severity,    // info|warning|error|critical
    string Value,
    string? Unit = null,
    string? Hint = null,
    string? Owner = null,
    string? Component = null,
    string? Kind = null  // live|ready|diag
);
