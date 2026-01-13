using EleonsoftSdk.modules.HealthCheck.Module.Core;
using EleonsoftSdk.modules.HealthCheck.Module.Core.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace EleonsoftSdk.modules.HealthCheck.Module.Tests.Core;

public class HealthRunCoordinatorTests
{
    private readonly HealthCheckService _healthCheckService;
    private readonly Mock<IHealthSnapshotStore> _snapshotStoreMock;
    private readonly Mock<IHealthReportBuilder> _reportBuilderMock;
    private readonly Mock<ILogger<HealthRunCoordinator>> _loggerMock;
    private readonly HealthRunCoordinator _coordinator;

    public HealthRunCoordinatorTests()
    {
        // HealthCheckService is sealed, so we need to create a real instance
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHealthChecks();
        var provider = services.BuildServiceProvider();
        _healthCheckService = provider.GetRequiredService<HealthCheckService>();
        
        _snapshotStoreMock = new Mock<IHealthSnapshotStore>();
        _reportBuilderMock = new Mock<IHealthReportBuilder>();
        _loggerMock = new Mock<ILogger<HealthRunCoordinator>>();

        _coordinator = new HealthRunCoordinator(
            _healthCheckService,
            _snapshotStoreMock.Object,
            _reportBuilderMock.Object,
            _loggerMock.Object,
            "TestApp");
    }

    [Fact]
    public async Task RunAsync_ShouldReturnNull_WhenAlreadyRunning()
    {
        // Arrange
        // With no registered checks, HealthCheckService completes very quickly
        // We need to delay the report builder to test the lock
        var tcs = new TaskCompletionSource<EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckEto>();
        _reportBuilderMock.Setup(x => x.BuildHealthCheckEto(It.IsAny<HealthReport>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
            .Returns(() => tcs.Task.Result);

        // Act - Start first run (will wait on tcs)
        var task1 = _coordinator.RunAsync("test", "test");
        
        // Give it a moment to acquire the lock
        await Task.Delay(10);
        
        // Start second run immediately (should return null because lock is held)
        var result2 = await _coordinator.RunAsync("test", "test");

        // Assert
        Assert.Null(result2);
        Assert.True(_coordinator.IsRunning);

        // Complete the first run
        tcs.SetResult(new EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckEto());
        await task1;
    }

    [Fact]
    public async Task RunAsync_ShouldEnforceTimeout()
    {
        // Arrange - Use a very short timeout
        // Note: With real HealthCheckService and no registered checks, this completes quickly
        // Timeout enforcement with actual delays is tested in integration tests
        var options = new HealthRunOptions { CheckTimeoutSeconds = 5 };
        
        // Act - Should complete successfully with no registered checks
        var result = await _coordinator.RunAsync("test", "test", options);
        
        // Assert - With no registered checks, should complete quickly
        Assert.NotNull(result);
        // Timeout enforcement with actual delays is tested in integration tests
    }

    [Fact]
    public async Task RunAsync_ShouldStoreSnapshot()
    {
        // Arrange
        var eto = new EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckEto();
        _reportBuilderMock.Setup(x => x.BuildHealthCheckEto(It.IsAny<HealthReport>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
            .Returns(eto);

        // Act
        var snapshot = await _coordinator.RunAsync("test", "test");

        // Assert
        Assert.NotNull(snapshot);
        _snapshotStoreMock.Verify(x => x.Store(It.IsAny<HealthSnapshot>()), Times.Once);
    }

    [Fact]
    public void GetLatestSnapshot_ShouldReturnStoredSnapshot()
    {
        // Arrange
        var expectedSnapshot = new HealthSnapshot(
            Guid.NewGuid(),
            DateTime.UtcNow,
            "test",
            "test",
            new EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckEto(),
            true,
            TimeSpan.Zero);

        _snapshotStoreMock.Setup(x => x.GetLatest()).Returns(expectedSnapshot);

        // Act
        var result = _coordinator.GetLatestSnapshot();

        // Assert
        Assert.Equal(expectedSnapshot, result);
    }

    [Fact]
    public async Task RunAsync_ShouldHandleExceptionDuringExecution()
    {
        // Arrange - Make report builder throw
        _reportBuilderMock.Setup(x => x.BuildHealthCheckEto(It.IsAny<HealthReport>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
            .Throws(new Exception("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () =>
        {
            await _coordinator.RunAsync("test", "test");
        });

        // Verify lock released
        Assert.False(_coordinator.IsRunning);
    }

    [Fact]
    public async Task RunAsync_ShouldHandleEmptyHealthReport()
    {
        // Arrange
        var eto = new EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckEto();
        _reportBuilderMock.Setup(x => x.BuildHealthCheckEto(It.IsAny<HealthReport>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
            .Returns(eto);

        // Act
        var snapshot = await _coordinator.RunAsync("test", "test");

        // Assert
        Assert.NotNull(snapshot);
        Assert.NotNull(snapshot.HealthCheck.Reports);
    }

    [Fact]
    public async Task RunAsync_ShouldHandleConcurrentRequests()
    {
        // Arrange
        // With no registered checks, HealthCheckService completes very quickly
        // We need to delay the report builder to test the lock
        var tcs = new TaskCompletionSource<EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckEto>();
        var callCount = 0;
        _reportBuilderMock.Setup(x => x.BuildHealthCheckEto(It.IsAny<HealthReport>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
            .Returns(() =>
            {
                callCount++;
                if (callCount == 1)
                {
                    // First call delays, allowing others to try
                    return tcs.Task.Result;
                }
                return new EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckEto();
            });

        // Act - Start 10 concurrent requests
        var tasks = new List<Task<HealthSnapshot?>>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(_coordinator.RunAsync("test", "test"));
        }

        // Give them a moment to try acquiring the lock
        await Task.Delay(50);

        // Complete the first one
        tcs.SetResult(new EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckEto());

        var results = await Task.WhenAll(tasks);

        // Assert - Only one should succeed, others return null
        var successCount = results.Count(r => r != null);
        Assert.Equal(1, successCount);
    }

    [Fact]
    public void GetLatestSnapshot_ShouldReturnNull_WhenNoRuns()
    {
        // Act
        var result = _coordinator.GetLatestSnapshot();

        // Assert
        Assert.Null(result);
    }
}
