using EleonsoftSdk.modules.HealthCheck.Module.Core.Models;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EleonsoftSdk.modules.HealthCheck.Module.Core;

/// <summary>
/// Converts Microsoft HealthCheck HealthReport to ETO models and structured observations.
/// </summary>
public interface IHealthReportBuilder
{
    /// <summary>
    /// Builds a HealthCheckEto from a HealthReport.
    /// </summary>
    HealthCheckEto BuildHealthCheckEto(
        HealthReport healthReport,
        Guid healthCheckId,
        string type,
        string initiatorName,
        DateTime createdAt);

    /// <summary>
    /// Extracts structured observations from a HealthReport.
    /// </summary>
    IReadOnlyList<HealthObservation> ExtractObservations(HealthReport healthReport);

    /// <summary>
    /// Scrubs sensitive information from output based on privileged mode.
    /// </summary>
    HealthCheckEto ScrubSensitiveData(HealthCheckEto eto, bool isPrivileged);
}
