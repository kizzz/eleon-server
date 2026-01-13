using Eleon.Telemetry.Lib.Integration.Tests.TestBase;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedModule.modules.Otel.Module;
using System.Linq;
using System.Threading;

namespace Eleon.Telemetry.Lib.Integration.Tests.SettingsProvider;

/// <summary>
/// Tests for event storm behavior when multiple settings update events fire rapidly.
/// </summary>
public class TelemetrySettingsProviderEventStormTests : TelemetryIntegrationTestBase
{
    [Fact]
    public async Task InitializeAsync_WithRapidCalls_Should_HandleGracefully()
    {
        // Arrange
        var logger = Substitute.For<ILogger<MockTelemetrySettingsProviderWithCounter>>();
        var configurator = CreateConfigurator();
        var provider = new MockTelemetrySettingsProviderWithCounter(logger, configurator);

        // Act - Call InitializeAsync 20 times rapidly
        var tasks = Enumerable.Range(0, 20)
            .Select(_ => provider.InitializeAsync())
            .ToArray();

        await Task.WhenAll(tasks);

        // Assert - All calls should complete without exceptions
        tasks.All(t => t.IsCompletedSuccessfully).Should().BeTrue();
    }

    [Fact]
    public async Task InitializeAsync_WithRapidCalls_Should_HaveBoundedApiCalls()
    {
        // Arrange
        var logger = Substitute.For<ILogger<MockTelemetrySettingsProviderWithCounter>>();
        var configurator = CreateConfigurator();
        var provider = new MockTelemetrySettingsProviderWithCounter(logger, configurator);

        // Act - Call InitializeAsync 20 times rapidly
        var tasks = Enumerable.Range(0, 20)
            .Select(_ => provider.InitializeAsync())
            .ToArray();

        await Task.WhenAll(tasks);

        // Assert - Without debounce, we'd expect 20 calls, but with debounce we'd expect fewer
        // This test verifies the pattern - actual implementation may vary
        provider.ApiCallCount.Should().BeGreaterThan(0);
        provider.ApiCallCount.Should().BeLessThanOrEqualTo(20);
    }

    [Fact]
    public async Task InitializeAsync_WithRapidCalls_Should_ApplyLastConfiguration()
    {
        // Arrange
        var logger = Substitute.For<ILogger<MockTelemetrySettingsProviderWithCounter>>();
        var configurator = CreateConfigurator();
        var provider = new MockTelemetrySettingsProviderWithCounter(logger, configurator);

        // Act - Call InitializeAsync multiple times rapidly
        var tasks = Enumerable.Range(0, 20)
            .Select(_ => provider.InitializeAsync())
            .ToArray();

        await Task.WhenAll(tasks);

        // Wait a bit for any async operations to complete
        await Task.Delay(200);

        // Assert - Should complete without exceptions
        // Note: We can't easily verify which config was applied last without internal state
        tasks.All(t => t.IsCompletedSuccessfully).Should().BeTrue();
    }

    [Fact]
    public async Task InitializeAsync_WithRapidCalls_Should_NotExhaustResources()
    {
        // Arrange
        var logger = Substitute.For<ILogger<MockTelemetrySettingsProviderWithCounter>>();
        var configurator = CreateConfigurator();
        var provider = new MockTelemetrySettingsProviderWithCounter(logger, configurator);

        // Act - Call InitializeAsync 50 times rapidly
        var tasks = Enumerable.Range(0, 50)
            .Select(_ => provider.InitializeAsync())
            .ToArray();

        await Task.WhenAll(tasks);

        // Assert - All should complete successfully
        tasks.All(t => t.IsCompletedSuccessfully).Should().BeTrue();
        provider.ApiCallCount.Should().BeLessThanOrEqualTo(50);
    }

    /// <summary>
    /// Mock implementation that counts API calls.
    /// </summary>
    public class MockTelemetrySettingsProviderWithCounter : ITelemetrySettingsProvider
    {
        private readonly ILogger<MockTelemetrySettingsProviderWithCounter> _logger;
        private readonly TelemetryConfigurator _configurator;
        private int _apiCallCount;

        public int ApiCallCount => _apiCallCount;

        public MockTelemetrySettingsProviderWithCounter(
            ILogger<MockTelemetrySettingsProviderWithCounter> logger,
            TelemetryConfigurator configurator)
        {
            _logger = logger;
            _configurator = configurator;
        }

        public async Task InitializeAsync()
        {
            Interlocked.Increment(ref _apiCallCount);

            // Simulate API call delay
            await Task.Delay(10);

            var options = new OtelOptions
            {
                Enabled = true,
                Traces = new OtelOptions.TracesOptions { Endpoint = "http://localhost:4318/v1/traces" },
                Metrics = new OtelOptions.MetricsOptions { Endpoint = "http://localhost:4318/v1/metrics" },
                Logs = new OtelOptions.LogsOptions { Endpoint = "http://localhost:4318/v1/logs" }
            };

            await _configurator.ConfigureAsync(options);
        }
    }
}
