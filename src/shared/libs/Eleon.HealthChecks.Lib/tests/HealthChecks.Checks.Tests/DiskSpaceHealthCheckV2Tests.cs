using EleonsoftSdk.modules.HealthCheck.Module.Checks.Environment;
using EleonsoftSdk.modules.HealthCheck.Module.Checks.SystemCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.IO;
using Xunit;

namespace EleonsoftSdk.modules.HealthCheck.Module.Tests.Checks;

public class DiskSpaceHealthCheckV2Tests
{
    private readonly Mock<ILogger<DiskSpaceHealthCheckV2>> _loggerMock;

    public DiskSpaceHealthCheckV2Tests()
    {
        _loggerMock = new Mock<ILogger<DiskSpaceHealthCheckV2>>();
    }

    [Fact]
    public async Task ShouldHandleEmptyItems()
    {
        // Arrange
        var options = Options.Create(new DiskSpaceHealthCheckOptions
        {
            Items = new List<FilePathRule>()
        });
        var check = new DiskSpaceHealthCheckV2(options, _loggerMock.Object);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HealthStatus.Healthy, result.Status);
    }

    [Fact]
    public async Task ShouldHandleInaccessiblePaths()
    {
        // Arrange
        var options = Options.Create(new DiskSpaceHealthCheckOptions
        {
            Items = new List<FilePathRule>
            {
                new FilePathRule
                {
                    Path = "Z:\\NonExistentDrive\\Path",
                    MaxSizeMb = 1
                }
            }
        });
        var check = new DiskSpaceHealthCheckV2(options, _loggerMock.Object);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        // Should handle gracefully without crashing
    }

    [Fact]
    public async Task ShouldHonorCancellationToken()
    {
        // Arrange
        var tempPath = Path.GetTempPath();
        var options = Options.Create(new DiskSpaceHealthCheckOptions
        {
            Items = new List<FilePathRule>
            {
                new FilePathRule
                {
                    Path = tempPath,
                    MaxSizeMb = 1000000
                }
            }
        });
        var check = new DiskSpaceHealthCheckV2(options, _loggerMock.Object);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert - Cancellation should throw OperationCanceledException
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await check.CheckHealthAsync(
                new HealthCheckContext(),
                cts.Token);
        });
        // Should handle cancellation gracefully
    }

    [Fact]
    public async Task ShouldHandlePathTooLong()
    {
        // Arrange
        var longPath = "C:\\" + new string('A', 300); // Very long path
        var options = Options.Create(new DiskSpaceHealthCheckOptions
        {
            Items = new List<FilePathRule>
            {
                new FilePathRule
                {
                    Path = longPath,
                    MaxSizeMb = 1
                }
            }
        });
        var check = new DiskSpaceHealthCheckV2(options, _loggerMock.Object);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        // Should handle path too long gracefully
    }
}
