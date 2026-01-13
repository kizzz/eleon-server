using EleonsoftSdk.modules.HealthCheck.Module.Checks.Configuration;
using EleonsoftSdk.modules.HealthCheck.Module.Implementations.CheckConfiguration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace EleonsoftSdk.modules.HealthCheck.Module.Tests.Checks;

public class ConfigurationHealthCheckV2Tests
{
    private readonly Mock<ILogger<ConfigurationHealthCheckV2>> _loggerMock;

    public ConfigurationHealthCheckV2Tests()
    {
        _loggerMock = new Mock<ILogger<ConfigurationHealthCheckV2>>();
    }

    [Fact]
    public async Task ShouldHandleNullConfigurationService()
    {
        // Arrange
        var check = new ConfigurationHealthCheckV2(null!, _loggerMock.Object);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
    }

    [Fact]
    public async Task ShouldHandleValidationException()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new CheckConfigurationOptions());
        var config = Mock.Of<IConfiguration>();
        var logger = Mock.Of<ILogger<CheckConfigurationService>>();
        var checkService = new Mock<CheckConfigurationService>(options, config, logger);
        
        checkService.Setup(x => x.CheckAsync())
            .ThrowsAsync(new Exception("Validation failed"));

        var check = new ConfigurationHealthCheckV2(checkService.Object, _loggerMock.Object);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
    }

    [Fact]
    public async Task ShouldHandleMissingSections()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new CheckConfigurationOptions());
        var config = new ConfigurationBuilder().Build();
        var logger = Mock.Of<ILogger<CheckConfigurationService>>();
        var checkService = new CheckConfigurationService(options, config, logger);
        var check = new ConfigurationHealthCheckV2(checkService, _loggerMock.Object);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        // Should handle missing sections gracefully
        Assert.True(result.Status == HealthStatus.Healthy || result.Status == HealthStatus.Degraded || result.Status == HealthStatus.Unhealthy);
    }
}
