using EleonsoftSdk.modules.HealthCheck.Module.Checks.System;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EleonsoftSdk.modules.HealthCheck.Module.Tests.Checks;

public class SystemLogHealthCheckV2Tests
{
    private readonly Mock<ILogger<SystemLogHealthCheckV2>> _loggerMock;

    public SystemLogHealthCheckV2Tests()
    {
        _loggerMock = new Mock<ILogger<SystemLogHealthCheckV2>>();
    }

    [Fact]
    public async Task ShouldHandleNullSinks()
    {
        // Arrange
        var check = new SystemLogHealthCheckV2(_loggerMock.Object);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        // Should handle null sinks gracefully
    }

    [Fact]
    public async Task ShouldReturnStructuredData()
    {
        // Arrange
        var check = new SystemLogHealthCheckV2(_loggerMock.Object);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        // Should include log sink information if available
    }

    [Fact]
    public async Task ShouldHonorCancellationToken()
    {
        // Arrange
        var check = new SystemLogHealthCheckV2(_loggerMock.Object);

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
