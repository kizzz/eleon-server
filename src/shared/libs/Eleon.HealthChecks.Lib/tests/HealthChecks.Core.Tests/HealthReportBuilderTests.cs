using EleonsoftSdk.modules.HealthCheck.Module.Core;
using EleonsoftSdk.modules.HealthCheck.Module.Core.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EleonsoftSdk.modules.HealthCheck.Module.Tests.Core;

public class HealthReportBuilderTests
{
    private readonly Mock<ILogger<HealthReportBuilder>> _loggerMock;
    private readonly HealthReportBuilder _builder;

    public HealthReportBuilderTests()
    {
        _loggerMock = new Mock<ILogger<HealthReportBuilder>>();
        _builder = new HealthReportBuilder(_loggerMock.Object, "TestApp");
    }

    [Fact]
    public void BuildHealthCheckEto_ShouldMapHealthyStatus()
    {
        // Arrange
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["test"] = new HealthReportEntry(HealthStatus.Healthy, "OK", TimeSpan.Zero, null, new Dictionary<string, object>())
        };
        var report = new HealthReport(entries, HealthStatus.Healthy, TimeSpan.Zero);

        // Act
        var eto = _builder.BuildHealthCheckEto(report, Guid.NewGuid(), "test", "test", DateTime.UtcNow);

        // Assert
        Assert.Equal(EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants.HealthCheckStatus.OK, eto.Status);
        Assert.Single(eto.Reports);
        Assert.Equal("test", eto.Reports[0].CheckName);
    }

    [Fact]
    public void BuildHealthCheckEto_ShouldMapUnhealthyStatus()
    {
        // Arrange
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["test"] = new HealthReportEntry(HealthStatus.Unhealthy, "Failed", TimeSpan.Zero, null, new Dictionary<string, object>())
        };
        var report = new HealthReport(entries, HealthStatus.Unhealthy, TimeSpan.Zero);

        // Act
        var eto = _builder.BuildHealthCheckEto(report, Guid.NewGuid(), "test", "test", DateTime.UtcNow);

        // Assert
        Assert.Equal(EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants.HealthCheckStatus.Failed, eto.Status);
    }

    [Fact]
    public void ScrubSensitiveData_ShouldRemoveConnectionStrings_WhenNotPrivileged()
    {
        // Arrange
        var eto = new EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckEto
        {
            Reports = new List<EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckReportEto>
            {
                new EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckReportEto
                {
                    ExtraInformation = new List<EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck.ReportExtraInformationEto>
                    {
                        new EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck.ReportExtraInformationEto
                        {
                            Key = "ConnectionString",
                            Value = "Server=localhost;Database=test;Password=secret123"
                        }
                    }
                }
            }
        };

        // Act
        var scrubbed = _builder.ScrubSensitiveData(eto, isPrivileged: false);

        // Assert
        Assert.Equal("[REDACTED]", scrubbed.Reports[0].ExtraInformation[0].Value);
    }

    [Fact]
    public void ScrubSensitiveData_ShouldNotScrub_WhenPrivileged()
    {
        // Arrange
        var originalValue = "Server=localhost;Database=test;Password=secret123";
        var eto = new EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckEto
        {
            Reports = new List<EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckReportEto>
            {
                new EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckReportEto
                {
                    ExtraInformation = new List<EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck.ReportExtraInformationEto>
                    {
                        new EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck.ReportExtraInformationEto
                        {
                            Key = "ConnectionString",
                            Value = originalValue
                        }
                    }
                }
            }
        };

        // Act
        var scrubbed = _builder.ScrubSensitiveData(eto, isPrivileged: true);

        // Assert
        Assert.Equal(originalValue, scrubbed.Reports[0].ExtraInformation[0].Value);
    }

    [Fact]
    public void BuildHealthCheckEto_ShouldHandleNullData()
    {
        // Arrange
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["test"] = new HealthReportEntry(HealthStatus.Healthy, "OK", TimeSpan.Zero, null, null!)
        };
        var report = new HealthReport(entries, HealthStatus.Healthy, TimeSpan.Zero);

        // Act
        var eto = _builder.BuildHealthCheckEto(report, Guid.NewGuid(), "test", "test", DateTime.UtcNow);

        // Assert
        Assert.NotNull(eto);
        Assert.Single(eto.Reports);
    }

    [Fact]
    public void BuildHealthCheckEto_ShouldHandleExceptionInEntry()
    {
        // Arrange
        var exception = new Exception("Test exception");
        exception.Data.Add("StackTrace", "at Test()");
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["test"] = new HealthReportEntry(HealthStatus.Unhealthy, "Failed", TimeSpan.Zero, exception, new Dictionary<string, object>())
        };
        var report = new HealthReport(entries, HealthStatus.Unhealthy, TimeSpan.Zero);

        // Act
        var eto = _builder.BuildHealthCheckEto(report, Guid.NewGuid(), "test", "test", DateTime.UtcNow);

        // Assert
        Assert.NotNull(eto);
        var reportEntry = eto.Reports.First();
        Assert.Contains(reportEntry.ExtraInformation, i => i.Key == "Exception");
    }

    [Fact]
    public void ScrubSensitiveData_ShouldHandleNullReports()
    {
        // Arrange
        var eto = new EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckEto
        {
            Reports = null!
        };

        // Act
        var scrubbed = _builder.ScrubSensitiveData(eto, isPrivileged: false);

        // Assert
        Assert.NotNull(scrubbed);
    }

    [Fact]
    public void ScrubSensitiveData_ShouldScrubVariousSecretPatterns()
    {
        // Arrange
        var eto = new EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckEto
        {
            Reports = new List<EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckReportEto>
            {
                new EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckReportEto
                {
                    ExtraInformation = new List<EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck.ReportExtraInformationEto>
                    {
                        new EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck.ReportExtraInformationEto
                        {
                            Key = "ConnectionString",
                            Value = "Server=localhost;Password=secret123"
                        },
                        new EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck.ReportExtraInformationEto
                        {
                            Key = "ApiKey",
                            Value = "sk-1234567890abcdef"
                        },
                        new EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck.ReportExtraInformationEto
                        {
                            Key = "Token",
                            Value = "Bearer abc123"
                        },
                        new EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck.ReportExtraInformationEto
                        {
                            Key = "SafeData",
                            Value = "This is safe information"
                        }
                    }
                }
            }
        };

        // Act
        var scrubbed = _builder.ScrubSensitiveData(eto, isPrivileged: false);

        // Assert
        var connectionString = scrubbed.Reports[0].ExtraInformation.First(i => i.Key == "ConnectionString");
        var apiKey = scrubbed.Reports[0].ExtraInformation.First(i => i.Key == "ApiKey");
        var token = scrubbed.Reports[0].ExtraInformation.First(i => i.Key == "Token");
        var safeData = scrubbed.Reports[0].ExtraInformation.First(i => i.Key == "SafeData");

        Assert.Equal("[REDACTED]", connectionString.Value);
        Assert.Equal("[REDACTED]", apiKey.Value);
        Assert.Equal("[REDACTED]", token.Value);
        Assert.Equal("This is safe information", safeData.Value);
    }
}
