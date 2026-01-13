using EleonsoftSdk.modules.HealthCheck.Module.Core.Models;
using EleonsoftSdk.modules.HealthCheck.Module.Delivery;
using EleonsoftSdk.modules.HealthCheck.Module.General;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EleonsoftSdk.modules.HealthCheck.Module.Tests.Delivery;

public class HttpHealthPublisherTests
{
    private readonly Mock<IHealthCheckService> _serviceMock;
    private readonly Mock<ILogger<HttpHealthPublisher>> _loggerMock;
    private readonly HttpHealthPublisher _publisher;

    public HttpHealthPublisherTests()
    {
        _serviceMock = new Mock<IHealthCheckService>();
        _loggerMock = new Mock<ILogger<HttpHealthPublisher>>();
        _publisher = new HttpHealthPublisher(_serviceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task PublishAsync_ShouldReturnTrue_WhenServiceSucceeds()
    {
        // Arrange
        var snapshot = CreateSnapshot();
        _serviceMock.Setup(x => x.SendHealthCheckAsync(It.IsAny<EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckEto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HealthCheckResponse(true, Guid.NewGuid(), null));

        // Act
        var result = await _publisher.PublishAsync(snapshot, CancellationToken.None);

        // Assert
        Assert.True(result);
        _serviceMock.Verify(x => x.SendHealthCheckAsync(It.IsAny<EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckEto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_ShouldReturnFalse_WhenServiceFails()
    {
        // Arrange
        var snapshot = CreateSnapshot();
        _serviceMock.Setup(x => x.SendHealthCheckAsync(It.IsAny<EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckEto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HealthCheckResponse(false, Guid.Empty, "Error"));

        // Act
        var result = await _publisher.PublishAsync(snapshot, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task PublishAsync_ShouldHandleServiceException()
    {
        // Arrange
        var snapshot = CreateSnapshot();
        _serviceMock.Setup(x => x.SendHealthCheckAsync(It.IsAny<EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckEto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _publisher.PublishAsync(snapshot, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task PublishAsync_ShouldHandleCancellation()
    {
        // Arrange
        var snapshot = CreateSnapshot();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var result = await _publisher.PublishAsync(snapshot, cts.Token);

        // Assert
        // Should handle cancellation gracefully
        Assert.False(result);
    }

    [Fact]
    public void Name_ShouldReturnHttp()
    {
        // Assert
        Assert.Equal("Http", _publisher.Name);
    }

    private static HealthSnapshot CreateSnapshot()
    {
        return new HealthSnapshot(
            Guid.NewGuid(),
            DateTime.UtcNow,
            "test",
            "test",
            new EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck.HealthCheckEto(),
            true,
            TimeSpan.Zero);
    }
}
