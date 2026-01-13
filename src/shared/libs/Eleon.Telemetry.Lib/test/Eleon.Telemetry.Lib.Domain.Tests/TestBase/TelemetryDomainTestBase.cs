using Eleon.TestsBase.Lib.TestBase;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using SharedModule.modules.Otel.Module;

namespace Eleon.Telemetry.Lib.Domain.Tests.TestBase;

/// <summary>
/// Base class for Telemetry Lib domain unit tests.
/// Provides mocked dependencies and helpers for testing telemetry logic.
/// </summary>
public abstract class TelemetryDomainTestBase : MockingTestBase
{
    protected ILoggerFactory CreateMockLoggerFactory()
    {
        return Substitute.For<ILoggerFactory>();
    }

    protected IHostEnvironment CreateMockHostEnvironment(string environmentName = "Test")
    {
        var env = Substitute.For<IHostEnvironment>();
        env.EnvironmentName.Returns(environmentName);
        return env;
    }

    protected IOptions<OtelOptions> CreateMockOptions(OtelOptions? options = null)
    {
        var mockOptions = Substitute.For<IOptions<OtelOptions>>();
        mockOptions.Value.Returns(options ?? new OtelOptions());
        return mockOptions;
    }

    protected IServiceProvider CreateMockServiceProvider()
    {
        return Substitute.For<IServiceProvider>();
    }

    protected ILogger<TelemetryConfigurator> CreateMockConfiguratorLogger()
    {
        return Substitute.For<ILogger<TelemetryConfigurator>>();
    }
}
