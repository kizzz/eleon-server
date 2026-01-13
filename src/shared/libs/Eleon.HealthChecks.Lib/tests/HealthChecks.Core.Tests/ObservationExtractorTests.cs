using EleonsoftSdk.modules.HealthCheck.Module.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace EleonsoftSdk.modules.HealthCheck.Module.Tests.Core;

public class ObservationExtractorTests
{
    [Fact]
    public void ExtractFromReport_ShouldExtractAllDataItems()
    {
        // Arrange
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["check1"] = new HealthReportEntry(
                HealthStatus.Healthy,
                "OK",
                TimeSpan.Zero,
                null,
                new Dictionary<string, object>
                {
                    ["metric1"] = 100,
                    ["metric2"] = "value"
                }),
            ["check2"] = new HealthReportEntry(
                HealthStatus.Unhealthy,
                "Failed",
                TimeSpan.Zero,
                null,
                new Dictionary<string, object>
                {
                    ["error"] = "Something went wrong"
                })
        };
        var report = new HealthReport(entries, HealthStatus.Degraded, TimeSpan.Zero);

        // Act
        var observations = ObservationExtractor.ExtractFromReport(report);

        // Assert
        Assert.NotNull(observations);
        Assert.True(observations.Count >= 4); // At least 2 status + 2 description + data items
    }

    [Fact]
    public void ExtractFromReport_ShouldDetermineCorrectTypes()
    {
        // Arrange
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["test"] = new HealthReportEntry(
                HealthStatus.Healthy,
                "OK",
                TimeSpan.Zero,
                null,
                new Dictionary<string, object>
                {
                    ["numeric"] = "123.45",
                    ["json"] = "{\"key\":\"value\"}",
                    ["link"] = "https://example.com",
                    ["text"] = "plain text"
                })
        };
        var report = new HealthReport(entries, HealthStatus.Healthy, TimeSpan.Zero);

        // Act
        var observations = ObservationExtractor.ExtractFromReport(report);

        // Assert
        var numericObs = observations.FirstOrDefault(o => o.Key.Contains("numeric"));
        var jsonObs = observations.FirstOrDefault(o => o.Key.Contains("json"));
        var linkObs = observations.FirstOrDefault(o => o.Key.Contains("link"));
        var textObs = observations.FirstOrDefault(o => o.Key.Contains("text"));

        Assert.NotNull(numericObs);
        Assert.Equal("metric", numericObs.Type);
        
        Assert.NotNull(jsonObs);
        Assert.Equal("json", jsonObs.Type);
        
        Assert.NotNull(linkObs);
        Assert.Equal("link", linkObs.Type);
        
        Assert.NotNull(textObs);
        Assert.Equal("text", textObs.Type);
    }

    [Fact]
    public void ExtractFromReport_ShouldMapSeverityCorrectly()
    {
        // Arrange
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["healthy"] = new HealthReportEntry(HealthStatus.Healthy, "OK", TimeSpan.Zero, null, null),
            ["degraded"] = new HealthReportEntry(HealthStatus.Degraded, "Warning", TimeSpan.Zero, null, null),
            ["unhealthy"] = new HealthReportEntry(HealthStatus.Unhealthy, "Failed", TimeSpan.Zero, null, null),
            ["unhealthy_error"] = new HealthReportEntry(
                HealthStatus.Unhealthy,
                "Error",
                TimeSpan.Zero,
                null,
                new Dictionary<string, object> { ["error_key"] = "error value" })
        };
        var report = new HealthReport(entries, HealthStatus.Unhealthy, TimeSpan.Zero);

        // Act
        var observations = ObservationExtractor.ExtractFromReport(report);

        // Assert
        var healthyObs = observations.FirstOrDefault(o => o.Key.Contains("healthy.status"));
        var degradedObs = observations.FirstOrDefault(o => o.Key.Contains("degraded.status"));
        var unhealthyObs = observations.FirstOrDefault(o => o.Key.Contains("unhealthy.status") && !o.Key.Contains("error"));
        var errorObs = observations.FirstOrDefault(o => o.Key.Contains("error_key"));

        Assert.NotNull(healthyObs);
        Assert.Equal("info", healthyObs.Severity);

        Assert.NotNull(degradedObs);
        Assert.Equal("warning", degradedObs.Severity);

        Assert.NotNull(unhealthyObs);
        Assert.Equal("error", unhealthyObs.Severity);

        Assert.NotNull(errorObs);
        Assert.Equal("critical", errorObs.Severity);
    }

    [Fact]
    public void ExtractFromReport_ShouldHandleNullValues()
    {
        // Arrange
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["test"] = new HealthReportEntry(
                HealthStatus.Healthy,
                "OK",
                TimeSpan.Zero,
                null,
                new Dictionary<string, object>
                {
                    ["null_value"] = null!
                })
        };
        var report = new HealthReport(entries, HealthStatus.Healthy, TimeSpan.Zero);

        // Act
        var observations = ObservationExtractor.ExtractFromReport(report);

        // Assert
        var nullObs = observations.FirstOrDefault(o => o.Key.Contains("null_value"));
        Assert.NotNull(nullObs);
        Assert.Equal("null", nullObs.Value);
    }

    [Fact]
    public void ExtractFromReport_ShouldHandleEmptyReport()
    {
        // Arrange
        var report = new HealthReport(
            new Dictionary<string, HealthReportEntry>(),
            HealthStatus.Healthy,
            TimeSpan.Zero);

        // Act
        var observations = ObservationExtractor.ExtractFromReport(report);

        // Assert
        Assert.NotNull(observations);
        Assert.Empty(observations);
    }
}
