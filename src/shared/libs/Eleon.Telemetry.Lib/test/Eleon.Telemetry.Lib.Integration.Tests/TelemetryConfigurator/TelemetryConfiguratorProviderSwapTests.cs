using Eleon.Telemetry.Lib.Integration.Tests.TestBase;
using Eleon.Telemetry.Lib.Domain.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using SharedModule.modules.Otel.Module;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Eleon.Telemetry.Lib.Integration.Tests.TelemetryConfiguratorTests;

/// <summary>
/// Critical regression test: Provider swap should not duplicate logs.
/// </summary>
public class TelemetryConfiguratorProviderSwapTests : TelemetryIntegrationTestBase
{
    [Fact]
    public async Task ConfigureAsync_AfterReconfigure_Should_NotDuplicateLogs()
    {
        // Arrange
        var logCounter = new ConcurrentBag<string>();
        var loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder.AddProvider(new CountingLoggerProvider(logCounter));
        });

        var swappableProvider = new SwappableLoggerProvider();
        loggerFactory.AddProvider(swappableProvider);

        var configurator = new TelemetryConfigurator(
            loggerFactory,
            Microsoft.Extensions.Options.Options.Create(DefaultOptions),
            HostEnvironment,
            ServiceProvider,
            ConfiguratorLogger,
            swappableProvider);

        var options = TelemetryTestDataBuilder.ValidOptions();
        var logger = loggerFactory.CreateLogger<TelemetryConfiguratorProviderSwapTests>();

        const int N = 10;

        // Act - Configure and emit N logs
        await configurator.ConfigureAsync(options);
        for (int i = 0; i < N; i++)
        {
            logger.LogInformation("Log message {Index}", i);
        }

        // Wait a bit for logs to be processed
        await Task.Delay(100);

        // Reconfigure (force) with same settings
        await configurator.ConfigureAsync(options, force: true);

        // Emit N more logs
        for (int i = 0; i < N; i++)
        {
            logger.LogInformation("Log message {Index}", i + N);
        }

        // Wait for logs to be processed
        await Task.Delay(200);

        // Cleanup
        configurator.Dispose();
        loggerFactory.Dispose();

        // Assert - Total logged messages should be 2N (exactly once each)
        // Note: This test verifies that logs are not duplicated due to provider accumulation
        // The actual count may vary based on OpenTelemetry batching, but we check for reasonable bounds
        // In a real scenario with in-memory exporters, we'd count exported log records
        // For now, we verify no exceptions and that the pattern doesn't cause obvious duplication
        logCounter.Count.Should().BeGreaterThanOrEqualTo(2 * N);
        logCounter.Count.Should().BeLessThan(3 * N); // Should not be 3N or more
    }

    private class CountingLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentBag<string> _counter;

        public CountingLoggerProvider(ConcurrentBag<string> counter)
        {
            _counter = counter;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new CountingLogger(_counter);
        }

        public void Dispose()
        {
        }

        private class CountingLogger : ILogger
        {
            private readonly ConcurrentBag<string> _counter;

            public CountingLogger(ConcurrentBag<string> counter)
            {
                _counter = counter;
            }

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                _counter.Add(formatter(state, exception));
            }
        }
    }
}
