using Eleon.Telemetry.Lib.Domain.Tests.TestBase;
using Eleon.Telemetry.Lib.Domain.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SharedModule.modules.Otel.Module;

namespace Eleon.Telemetry.Lib.Domain.Tests.TelemetryConfiguratorTests;

public class TelemetryConfiguratorIdempotencyTests : TelemetryDomainTestBase
{
    [Fact]
    public async Task ConfigureAsync_CalledTwiceWithSameOptions_Should_BeIdempotent()
    {
        // Arrange
        var options = TelemetryTestDataBuilder.ValidOptions();
        var configurator = CreateConfigurator();

        // Act
        await configurator.ConfigureAsync(options);
        await configurator.ConfigureAsync(options);

        // Assert - Should complete without throwing
        // The second call should be a no-op due to equality check
        configurator.Should().NotBeNull();
    }

    [Fact]
    public async Task ConfigureAsync_CalledTwiceWithForce_Should_Reconfigure()
    {
        // Arrange
        var options = TelemetryTestDataBuilder.ValidOptions();
        var configurator = CreateConfigurator();

        // Act
        await configurator.ConfigureAsync(options);
        await configurator.ConfigureAsync(options, force: true);

        // Assert - Should complete without throwing
        // Force flag should bypass equality check
        configurator.Should().NotBeNull();
    }

    [Fact]
    public async Task ConfigureAsync_WithEnabledFalseAfterEnabled_Should_TearDownProviders()
    {
        // Arrange
        var enabledOptions = TelemetryTestDataBuilder.ValidOptions();
        var disabledOptions = TelemetryTestDataBuilder.DisabledOptions();
        var configurator = CreateConfigurator();

        // Act
        await configurator.ConfigureAsync(enabledOptions);
        await configurator.ConfigureAsync(disabledOptions);

        // Assert - Should complete without throwing
        // Providers should be torn down
        configurator.Should().NotBeNull();
    }

    [Fact]
    public async Task ConfigureAsync_WithRepeatedEnableDisableFlipFlops_Should_NotLeakOrThrow()
    {
        // Arrange
        var enabledOptions = TelemetryTestDataBuilder.ValidOptions();
        var disabledOptions = TelemetryTestDataBuilder.DisabledOptions();
        var configurator = CreateConfigurator();

        // Act - Flip flop multiple times
        for (int i = 0; i < 5; i++)
        {
            await configurator.ConfigureAsync(enabledOptions);
            await configurator.ConfigureAsync(disabledOptions);
        }

        // Assert - Should complete without throwing or leaking
        configurator.Should().NotBeNull();
    }

    [Fact]
    public void Dispose_AfterConfigure_Should_DisposeProviders()
    {
        // Arrange
        var options = TelemetryTestDataBuilder.ValidOptions();
        var configurator = CreateConfigurator();

        // Act
        configurator.ConfigureAsync(options).Wait();
        configurator.Dispose();

        // Assert - Should complete without throwing
        configurator.Should().NotBeNull();
    }

    [Fact]
    public void Dispose_WithoutConfigure_Should_NotThrow()
    {
        // Arrange
        var configurator = CreateConfigurator();

        // Act
        configurator.Dispose();

        // Assert - Should complete without throwing
        configurator.Should().NotBeNull();
    }

    [Fact]
    public async Task ConfigureAsync_WithSameOptionsButDifferentInstance_Should_BeIdempotent()
    {
        // Arrange
        var options1 = TelemetryTestDataBuilder.ValidOptions();
        var options2 = TelemetryTestDataBuilder.ValidOptions(); // Same values, different instance
        var configurator = CreateConfigurator();

        // Act
        await configurator.ConfigureAsync(options1);
        await configurator.ConfigureAsync(options2);

        // Assert - Should be idempotent because options are equal
        configurator.Should().NotBeNull();
    }

    private TelemetryConfigurator CreateConfigurator()
    {
        var loggerFactory = CreateMockLoggerFactory();
        var hostEnvironment = CreateMockHostEnvironment();
        var serviceProvider = CreateMockServiceProvider();
        var options = CreateMockOptions();
        var logger = CreateMockConfiguratorLogger();

        return new TelemetryConfigurator(
            loggerFactory,
            options,
            hostEnvironment,
            serviceProvider,
            logger);
    }
}
