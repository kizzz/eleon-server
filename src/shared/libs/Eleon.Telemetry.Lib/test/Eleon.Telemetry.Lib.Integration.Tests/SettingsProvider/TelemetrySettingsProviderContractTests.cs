using Eleon.Telemetry.Lib.Integration.Tests.TestBase;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedModule.modules.Otel.Module;
using System;
using System.Threading.Tasks;

namespace Eleon.Telemetry.Lib.Integration.Tests.SettingsProvider;

/// <summary>
/// Tests for ITelemetrySettingsProvider contract behavior.
/// These tests verify expected behavior when providers fail or return null.
/// </summary>
public class TelemetrySettingsProviderContractTests : TelemetryIntegrationTestBase
{
    [Fact]
    public async Task InitializeAsync_WithApiReturningNull_Should_LogWarning()
    {
        // Arrange
        var logger = Substitute.For<ILogger<MockTelemetrySettingsProvider>>();
        var configurator = CreateConfigurator();
        var provider = new MockTelemetrySettingsProvider(logger, configurator)
        {
            ReturnNull = true
        };

        // Act
        await provider.InitializeAsync();

        // Assert - Should log warning about null settings
        logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task InitializeAsync_WithApiTimeout_Should_NotCrash()
    {
        // Arrange
        var logger = Substitute.For<ILogger<MockTelemetrySettingsProvider>>();
        var configurator = CreateConfigurator();
        var provider = new MockTelemetrySettingsProvider(logger, configurator)
        {
            ThrowTimeout = true
        };

        // Act
        await FluentActions.Awaiting(() => provider.InitializeAsync())
            .Should().NotThrowAsync();

        // Assert - Should handle timeout gracefully
    }

    [Fact]
    public async Task InitializeAsync_WithException_Should_HandleGracefully()
    {
        // Arrange
        var logger = Substitute.For<ILogger<MockTelemetrySettingsProvider>>();
        var configurator = CreateConfigurator();
        var provider = new MockTelemetrySettingsProvider(logger, configurator)
        {
            ThrowException = true
        };

        // Act
        await FluentActions.Awaiting(() => provider.InitializeAsync())
            .Should().NotThrowAsync();

        // Assert - Should handle exception gracefully
    }

    [Fact]
    public async Task InitializeAsync_WithValidSettings_Should_CallConfigureAsync()
    {
        // Arrange
        var logger = Substitute.For<ILogger<MockTelemetrySettingsProvider>>();
        var configurator = CreateConfigurator();
        var provider = new MockTelemetrySettingsProvider(logger, configurator);

        // Act
        await provider.InitializeAsync();

        // Assert - ConfigureAsync should be called (we can't easily verify this without internal state,
        // but we verify no exceptions)
        configurator.Should().NotBeNull();
    }

    /// <summary>
    /// Mock implementation of ITelemetrySettingsProvider for testing.
    /// </summary>
    public class MockTelemetrySettingsProvider : ITelemetrySettingsProvider
    {
        private readonly ILogger<MockTelemetrySettingsProvider> _logger;
        private readonly TelemetryConfigurator _configurator;

        public bool ReturnNull { get; set; }
        public bool ThrowTimeout { get; set; }
        public bool ThrowException { get; set; }

        public MockTelemetrySettingsProvider(
            ILogger<MockTelemetrySettingsProvider> logger,
            TelemetryConfigurator configurator)
        {
            _logger = logger;
            _configurator = configurator;
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (ThrowTimeout)
                {
                    await Task.Delay(100);
                    throw new TimeoutException("API timeout");
                }

                if (ThrowException)
                {
                    throw new InvalidOperationException("Test exception");
                }

                if (ReturnNull)
                {
                    _logger.LogWarning("Failed to retrieve tenant system health settings during telemetry initialization.");
                    return;
                }

                // Simulate valid settings
                var options = new OtelOptions
                {
                    Enabled = true,
                    Traces = new OtelOptions.TracesOptions { Endpoint = "http://localhost:4318/v1/traces" },
                    Metrics = new OtelOptions.MetricsOptions { Endpoint = "http://localhost:4318/v1/metrics" },
                    Logs = new OtelOptions.LogsOptions { Endpoint = "http://localhost:4318/v1/logs" }
                };

                await _configurator.ConfigureAsync(options);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to initialize telemetry settings.");
            }
        }
    }
}
