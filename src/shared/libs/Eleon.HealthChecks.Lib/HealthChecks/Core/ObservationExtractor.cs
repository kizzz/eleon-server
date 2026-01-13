using EleonsoftSdk.modules.HealthCheck.Module.Core.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EleonsoftSdk.modules.HealthCheck.Module.Core;

/// <summary>
/// Extracts structured observations from HealthCheckResult.Data dictionaries.
/// </summary>
public static class ObservationExtractor
{
    /// <summary>
    /// Extracts observations from a HealthReport.
    /// </summary>
    public static IReadOnlyList<HealthObservation> ExtractFromReport(HealthReport report)
    {
        var observations = new List<HealthObservation>();

        foreach (var entry in report.Entries)
        {
            var checkName = entry.Key;
            var result = entry.Value;

            // Extract from Data dictionary
            if (result.Data != null)
            {
                foreach (var kvp in result.Data)
                {
                    var key = kvp.Key;
                    var value = kvp.Value?.ToString() ?? "null";

                    // Determine type
                    var type = DetermineType(value);
                    var severity = DetermineSeverity(result.Status, key);

                    observations.Add(new HealthObservation(
                        Key: $"{checkName}.{key}",
                        Type: type,
                        Severity: severity,
                        Value: value,
                        Component: checkName
                    ));
                }
            }

            // Add status observation
            observations.Add(new HealthObservation(
                Key: $"{checkName}.status",
                Type: "text",
                Severity: DetermineSeverity(result.Status, "status"),
                Value: result.Status.ToString(),
                Component: checkName
            ));

            // Add description if present
            if (!string.IsNullOrWhiteSpace(result.Description))
            {
                observations.Add(new HealthObservation(
                    Key: $"{checkName}.description",
                    Type: "text",
                    Severity: DetermineSeverity(result.Status, "description"),
                    Value: result.Description,
                    Component: checkName
                ));
            }
        }

        return observations;
    }

    private static string DetermineType(string value)
    {
        if (double.TryParse(value, out _))
            return "metric";
        if (value.StartsWith("{") || value.StartsWith("["))
            return "json";
        if (Uri.TryCreate(value, UriKind.Absolute, out _))
            return "link";
        return "text";
    }

    private static string DetermineSeverity(HealthStatus status, string key)
    {
        if (status == HealthStatus.Unhealthy)
            return key.Contains("error", StringComparison.OrdinalIgnoreCase) ? "critical" : "error";
        if (status == HealthStatus.Degraded)
            return "warning";
        return "info";
    }
}
