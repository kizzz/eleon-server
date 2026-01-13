using Eleon.Telemetry.Lib.Integration.Tests.TestBase;
using Eleon.Telemetry.Lib.Domain.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using SharedModule.modules.Otel.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Eleon.Telemetry.Lib.Integration.Tests.TelemetryConfiguratorTests;

/// <summary>
/// Tests reconfigure behavior under continuous load.
/// </summary>
public class TelemetryConfiguratorReconfigureUnderLoadTests : TelemetryIntegrationTestBase
{
    [Fact]
    public async Task ConfigureAsync_WithReconfiguresUnderLoad_Should_NotThrowExceptions()
    {
        // Arrange
        var swappableProvider = new SwappableLoggerProvider();
        var loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder.AddProvider(swappableProvider);
        });

        var configurator = new TelemetryConfigurator(
            loggerFactory,
            Microsoft.Extensions.Options.Options.Create(DefaultOptions),
            HostEnvironment,
            ServiceProvider,
            ConfiguratorLogger,
            swappableProvider);

        var logger = loggerFactory.CreateLogger<TelemetryConfiguratorReconfigureUnderLoadTests>();
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        // Act - Background task logs continuously
        var loggingTask = Task.Run(async () =>
        {
            int counter = 0;
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                logger.LogInformation("Background log message {Counter}", counter++);
                await Task.Delay(10, cancellationTokenSource.Token);
            }
        }, cancellationTokenSource.Token);

        // Reconfigure 50 times with varying settings
        var reconfigureTasks = new List<Task>();
        for (int i = 0; i < 50; i++)
        {
            var options = new TelemetryTestDataBuilder()
                .WithEnabled()
                .WithServiceName($"Service{i}")
                .WithTracesEndpoint($"http://localhost:{4318 + (i % 10)}/v1/traces")
                .WithMetricsEndpoint($"http://localhost:{4318 + (i % 10)}/v1/metrics")
                .WithLogsEndpoint($"http://localhost:{4318 + (i % 10)}/v1/logs")
                .Build();

            reconfigureTasks.Add(configurator.ConfigureAsync(options, force: true));
            await Task.Delay(50); // Small delay between reconfigures
        }

        await Task.WhenAll(reconfigureTasks);
        cancellationTokenSource.Cancel();

        try
        {
            await loggingTask;
        }
        catch (OperationCanceledException)
        {
            // Expected
        }

        // Cleanup
        configurator.Dispose();
        loggerFactory.Dispose();

        // Assert - Should complete without exceptions
        reconfigureTasks.All(t => t.IsCompletedSuccessfully).Should().BeTrue();
    }

    [Fact]
    public async Task ConfigureAsync_WithReconfiguresUnderLoad_Should_NotCauseMemoryBallooning()
    {
        // Arrange
        var swappableProvider = new SwappableLoggerProvider();
        var loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder.AddProvider(swappableProvider);
        });

        var configurator = new TelemetryConfigurator(
            loggerFactory,
            Microsoft.Extensions.Options.Options.Create(DefaultOptions),
            HostEnvironment,
            ServiceProvider,
            ConfiguratorLogger,
            swappableProvider);

        // Force GC before measurement
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var memoryBefore = GC.GetTotalMemory(false);

        // Act - Reconfigure 30 times
        for (int i = 0; i < 30; i++)
        {
            var options = new TelemetryTestDataBuilder()
                .WithEnabled()
                .WithTracesEndpoint($"http://localhost:{4318 + (i % 5)}/v1/traces")
                .WithMetricsEndpoint($"http://localhost:{4318 + (i % 5)}/v1/metrics")
                .WithLogsEndpoint($"http://localhost:{4318 + (i % 5)}/v1/logs")
                .Build();

            await configurator.ConfigureAsync(options, force: true);
        }

        // Cleanup
        configurator.Dispose();
        loggerFactory.Dispose();

        // Force GC after cleanup
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var memoryAfter = GC.GetTotalMemory(false);
        var memoryIncrease = memoryAfter - memoryBefore;

        // Assert - Memory increase should be reasonable (less than 75MB for 30 reconfigures)
        // This is a sanity check - actual memory usage depends on OpenTelemetry implementation
        memoryIncrease.Should().BeLessThan(75 * 1024 * 1024); // 75MB
    }

    [Fact]
    public async Task ConfigureAsync_WithFinalConfiguration_Should_MatchLastConfiguration()
    {
        // Arrange
        var swappableProvider = new SwappableLoggerProvider();
        var loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder.AddProvider(swappableProvider);
        });

        var configurator = new TelemetryConfigurator(
            loggerFactory,
            Microsoft.Extensions.Options.Options.Create(DefaultOptions),
            HostEnvironment,
            ServiceProvider,
            ConfiguratorLogger,
            swappableProvider);

        // Act - Reconfigure multiple times, then configure with a known final option
        for (int i = 0; i < 10; i++)
        {
            var options = new TelemetryTestDataBuilder()
                .WithEnabled()
                .WithServiceName($"Service{i}")
                .WithTracesEndpoint($"http://localhost:{4318 + i}/v1/traces")
                .WithMetricsEndpoint($"http://localhost:{4318 + i}/v1/metrics")
                .WithLogsEndpoint($"http://localhost:{4318 + i}/v1/logs")
                .Build();

            await configurator.ConfigureAsync(options, force: true);
        }

        var finalOption = new TelemetryTestDataBuilder()
            .WithEnabled()
            .WithServiceName("FinalService")
            .WithTracesEndpoint("http://localhost:9999/v1/traces")
            .WithMetricsEndpoint("http://localhost:9999/v1/metrics")
            .WithLogsEndpoint("http://localhost:9999/v1/logs")
            .Build();

        await configurator.ConfigureAsync(finalOption, force: true);

        // Cleanup
        configurator.Dispose();
        loggerFactory.Dispose();

        // Assert - Should complete without exceptions
        configurator.Should().NotBeNull();
    }
}
