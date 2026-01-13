using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.HealthCheck.Module.Core.Models;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using SharedModule.modules.Helpers.Module;

namespace EleonsoftSdk.modules.HealthCheck.Module.Core;

/// <summary>
/// Converts Microsoft HealthCheck HealthReport to ETO models.
/// </summary>
public class HealthReportBuilder : IHealthReportBuilder
{
    private readonly ILogger<HealthReportBuilder> _logger;
    private readonly string _applicationName;

    public HealthReportBuilder(ILogger<HealthReportBuilder> logger, string applicationName)
    {
        _logger = logger;
        _applicationName = applicationName;
    }

    public HealthCheckEto BuildHealthCheckEto(
        HealthReport healthReport,
        Guid healthCheckId,
        string type,
        string initiatorName,
        DateTime createdAt)
    {
        var reports = new List<HealthCheckReportEto>();

        foreach (var entry in healthReport.Entries)
        {
            var report = new HealthCheckReportEto
            {
                CheckName = entry.Key,
                Status = MapStatus(entry.Value.Status),
                Message = entry.Value.Description ?? entry.Value.Status.ToString(),
                HealthCheckId = healthCheckId,
                ServiceName = _applicationName,
                ServiceVersion = VersionHelper.Version,
                UpTime = StartupDiagnostics.GetUptime(),
                IsPublic = true, // Default to public, can be overridden by check registration
                ExtraInformation = new List<ReportExtraInformationEto>()
            };

            // Convert Data dictionary to ExtraInformation
            if (entry.Value.Data != null)
            {
                foreach (var kvp in entry.Value.Data)
                {
                    report.ExtraInformation.Add(new ReportExtraInformationEto
                    {
                        Key = kvp.Key,
                        Value = kvp.Value?.ToString() ?? "null",
                        Severity = MapSeverity(entry.Value.Status, kvp.Key),
                        Type = DetermineType(kvp.Value)
                    });
                }
            }

            // Add exception if present
            if (entry.Value.Exception != null)
            {
                report.ExtraInformation.Add(new ReportExtraInformationEto
                {
                    Key = "Exception",
                    Value = entry.Value.Exception.Message,
                    Severity = ReportInformationSeverity.Error,
                    Type = "Simple"
                });

                if (!string.IsNullOrWhiteSpace(entry.Value.Exception.StackTrace))
                {
                    report.ExtraInformation.Add(new ReportExtraInformationEto
                    {
                        Key = "StackTrace",
                        Value = entry.Value.Exception.StackTrace,
                        Severity = ReportInformationSeverity.Error,
                        Type = "Simple"
                    });
                }
            }

            reports.Add(report);
        }

        // Determine overall status
        var overallStatus = healthReport.Status == HealthStatus.Healthy
            ? HealthCheckStatus.OK
            : healthReport.Status == HealthStatus.Degraded
                ? HealthCheckStatus.OK // Degraded maps to OK for backward compatibility
                : HealthCheckStatus.Failed;

        return new HealthCheckEto
        {
            Id = healthCheckId,
            Type = type,
            InitiatorName = initiatorName,
            CreationTime = createdAt,
            Status = overallStatus,
            Reports = reports
        };
    }

    public IReadOnlyList<HealthObservation> ExtractObservations(HealthReport healthReport)
    {
        return ObservationExtractor.ExtractFromReport(healthReport);
    }

    public HealthCheckEto ScrubSensitiveData(HealthCheckEto eto, bool isPrivileged)
    {
        if (isPrivileged)
            return eto; // No scrubbing for privileged users

        // Create a scrubbed copy
        var scrubbed = new HealthCheckEto
        {
            Id = eto.Id,
            Type = eto.Type,
            InitiatorName = eto.InitiatorName,
            CreationTime = eto.CreationTime,
            Status = eto.Status,
            Reports = new List<HealthCheckReportEto>()
        };

        if (eto.Reports == null)
        {
            return scrubbed;
        }

        foreach (var report in eto.Reports)
        {
            var scrubbedReport = new HealthCheckReportEto
            {
                CheckName = report.CheckName,
                Status = report.Status,
                Message = report.Message,
                HealthCheckId = report.HealthCheckId,
                ServiceName = report.ServiceName,
                ServiceVersion = report.ServiceVersion,
                UpTime = report.UpTime,
                IsPublic = report.IsPublic,
                ExtraInformation = new List<ReportExtraInformationEto>()
            };

            foreach (var info in report.ExtraInformation)
            {
                // Scrub connection strings, tokens, stack traces
                if (ShouldScrub(info.Key, info.Value))
                {
                    scrubbedReport.ExtraInformation.Add(new ReportExtraInformationEto
                    {
                        Key = info.Key,
                        Value = "[REDACTED]",
                        Severity = info.Severity,
                        Type = info.Type
                    });
                }
                else
                {
                    scrubbedReport.ExtraInformation.Add(info);
                }
            }

            scrubbed.Reports.Add(scrubbedReport);
        }

        return scrubbed;
    }

    private static HealthCheckStatus MapStatus(HealthStatus status)
    {
        return status switch
        {
            HealthStatus.Healthy => HealthCheckStatus.OK,
            HealthStatus.Degraded => HealthCheckStatus.OK, // Map degraded to OK for backward compatibility
            HealthStatus.Unhealthy => HealthCheckStatus.Failed,
            _ => HealthCheckStatus.Failed
        };
    }

    private static ReportInformationSeverity MapSeverity(HealthStatus status, string key)
    {
        if (status == HealthStatus.Unhealthy)
            return key.Contains("error", StringComparison.OrdinalIgnoreCase) 
                ? ReportInformationSeverity.Error 
                : ReportInformationSeverity.Warning;
        if (status == HealthStatus.Degraded)
            return ReportInformationSeverity.Warning;
        return ReportInformationSeverity.Info;
    }

    private static string DetermineType(object? value)
    {
        if (value == null)
            return "Simple";
        if (value is string str)
        {
            if (str.StartsWith("{") || str.StartsWith("["))
                return "Json";
            if (Uri.TryCreate(str, UriKind.Absolute, out _))
                return "Link";
        }
        return "Simple";
    }

    private static bool ShouldScrub(string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var lowerKey = key.ToLowerInvariant();
        var lowerValue = value.ToLowerInvariant();

        // Scrub connection strings
        if (lowerKey.Contains("connection") || lowerKey.Contains("connectionstring"))
            return true;

        // Scrub tokens
        if (lowerKey.Contains("token") || lowerKey.Contains("password") || lowerKey.Contains("secret") || lowerKey.Contains("key"))
            return true;

        // Scrub stack traces (unless privileged)
        if (lowerKey.Contains("stacktrace") || lowerKey.Contains("exception"))
            return true;

        // Scrub if value looks like a connection string
        if (lowerValue.Contains("data source") || lowerValue.Contains("server=") || lowerValue.Contains("password="))
            return true;

        return false;
    }
}
