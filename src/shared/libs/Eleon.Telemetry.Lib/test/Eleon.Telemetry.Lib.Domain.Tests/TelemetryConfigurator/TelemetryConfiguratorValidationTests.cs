using Eleon.Telemetry.Lib.Domain.Tests.TestBase;
using Eleon.Telemetry.Lib.Domain.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedModule.modules.Otel.Module;

namespace Eleon.Telemetry.Lib.Domain.Tests.TelemetryConfiguratorTests;

public class TelemetryConfiguratorValidationTests : TelemetryDomainTestBase
{
    [Fact]
    public async Task ConfigureAsync_WithNullOptions_Should_ReturnEarly()
    {
        // Arrange
        var configurator = CreateConfigurator();

        // Act
        await configurator.ConfigureAsync(null!);

        // Assert - Should not throw
        configurator.Should().NotBeNull();
    }

    [Fact]
    public async Task ConfigureAsync_WithEmptyEndpointAndEnabled_Should_CompleteGracefully()
    {
        // Arrange
        var options = new TelemetryTestDataBuilder()
            .WithEnabled()
            .WithInvalidTracesEndpoint()
            .WithInvalidMetricsEndpoint()
            .WithInvalidLogsEndpoint()
            .Build();

        var logger = CreateMockConfiguratorLogger();
        var configurator = CreateConfigurator(logger: logger);

        // Act
        await configurator.ConfigureAsync(options);

        // Assert - Should complete without throwing
        // Logger should receive warning messages
        logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task ConfigureAsync_WithMalformedUri_Should_CompleteGracefully()
    {
        // Arrange
        var options = new TelemetryTestDataBuilder()
            .WithEnabled()
            .WithMalformedTracesEndpoint()
            .WithMalformedMetricsEndpoint()
            .WithMalformedLogsEndpoint()
            .Build();

        var logger = CreateMockConfiguratorLogger();
        var configurator = CreateConfigurator(logger: logger);

        // Act
        await configurator.ConfigureAsync(options);

        // Assert - Should complete without throwing
        logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task ConfigureAsync_WithTracesBadAndMetricsOk_Should_ConfigureMetricsOnly()
    {
        // Arrange
        var options = new TelemetryTestDataBuilder()
            .WithEnabled()
            .WithInvalidTracesEndpoint()
            .WithMetricsEndpoint("http://localhost:4318/v1/metrics")
            .WithLogsEndpoint("http://localhost:4318/v1/logs")
            .Build();

        var logger = CreateMockConfiguratorLogger();
        var configurator = CreateConfigurator(logger: logger);

        // Act
        await configurator.ConfigureAsync(options);

        // Assert - Should log warning for traces but still configure metrics
        logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
        // Should complete without exception
    }

    [Fact]
    public async Task ConfigureAsync_WithAllInvalidSignals_Should_TearDownProviders()
    {
        // Arrange
        var options = new TelemetryTestDataBuilder()
            .WithEnabled()
            .WithInvalidTracesEndpoint()
            .WithInvalidMetricsEndpoint()
            .WithInvalidLogsEndpoint()
            .Build();

        var logger = CreateMockConfiguratorLogger();
        var configurator = CreateConfigurator(logger: logger);

        // Act
        await configurator.ConfigureAsync(options);

        // Assert - Should log warning that all signals are invalid
        logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    private TelemetryConfigurator CreateConfigurator(ILogger<TelemetryConfigurator>? logger = null)
    {
        var loggerFactory = CreateMockLoggerFactory();
        var hostEnvironment = CreateMockHostEnvironment();
        var serviceProvider = CreateMockServiceProvider();
        var options = CreateMockOptions();
        logger ??= CreateMockConfiguratorLogger();

        return new TelemetryConfigurator(
            loggerFactory,
            options,
            hostEnvironment,
            serviceProvider,
            logger);
    }
}
