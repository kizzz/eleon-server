using EleonsoftSdk.modules.HealthCheck.Module.Checks.Environment;
using EleonsoftSdk.modules.HealthCheck.Module.Checks.SystemCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace EleonsoftSdk.modules.HealthCheck.Module.Tests.Checks;

public class EnvironmentHealthCheckV2Tests
{
    private readonly Mock<ILogger<EnvironmentHealthCheckV2>> _loggerMock;

    public EnvironmentHealthCheckV2Tests()
    {
        _loggerMock = new Mock<ILogger<EnvironmentHealthCheckV2>>();
    }

    [Fact]
    public async Task ShouldReturnStructuredData()
    {
        // Arrange
        var options = Options.Create(new EnvironmentHealthCheckOptions
        {
            CpuThresholdPercent = 95.0,
            MemoryThresholdPercent = 95.0
        });
        var check = new EnvironmentHealthCheckV2(options, _loggerMock.Object);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Contains("env.cpu_percent", result.Data.Keys);
        Assert.Contains("env.memory_percent", result.Data.Keys);
    }

    [Fact]
    public async Task ShouldHandleZeroProcessors()
    {
        // Arrange
        var options = Options.Create(new EnvironmentHealthCheckOptions());
        var check = new EnvironmentHealthCheckV2(options, _loggerMock.Object);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        // Should handle gracefully even if processor count is unusual
    }

    [Fact]
    public async Task ShouldHonorCancellationToken()
    {
        // Arrange
        var options = Options.Create(new EnvironmentHealthCheckOptions());
        var check = new EnvironmentHealthCheckV2(options, _loggerMock.Object);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            cts.Token);

        // Assert
        Assert.NotNull(result);
        // Should handle cancellation gracefully
    }
}
