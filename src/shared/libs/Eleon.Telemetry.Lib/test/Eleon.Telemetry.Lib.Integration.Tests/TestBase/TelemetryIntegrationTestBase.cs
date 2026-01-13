using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedModule.modules.Otel.Module;
using System;
using System.Reflection;

namespace Eleon.Telemetry.Lib.Integration.Tests.TestBase;

/// <summary>
/// Base class for Telemetry Lib integration tests.
/// Provides helpers for creating TelemetryConfigurator with real dependencies.
/// </summary>
public abstract class TelemetryIntegrationTestBase : IDisposable
{
    protected ILoggerFactory LoggerFactory { get; }
    protected IHostEnvironment HostEnvironment { get; }
    protected IServiceProvider ServiceProvider { get; }
    protected ILogger<TelemetryConfigurator> ConfiguratorLogger { get; }
    protected OtelOptions DefaultOptions { get; }

    protected TelemetryIntegrationTestBase()
    {
        LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole());
        HostEnvironment = new TestHostEnvironment { EnvironmentName = "Test" };
        ServiceProvider = new ServiceCollection().BuildServiceProvider();
        ConfiguratorLogger = LoggerFactory.CreateLogger<TelemetryConfigurator>();
        DefaultOptions = new OtelOptions
        {
            Enabled = true,
            ServiceName = "TestService",
            Traces = new OtelOptions.TracesOptions { Endpoint = "http://localhost:4318/v1/traces" },
            Metrics = new OtelOptions.MetricsOptions { Endpoint = "http://localhost:4318/v1/metrics" },
            Logs = new OtelOptions.LogsOptions { Endpoint = "http://localhost:4318/v1/logs" }
        };
    }

    protected TelemetryConfigurator CreateConfigurator(OtelOptions? options = null, SwappableLoggerProvider? swappableLoggerProvider = null)
    {
        var opts = Options.Create(options ?? DefaultOptions);
        return new TelemetryConfigurator(
            LoggerFactory,
            opts,
            HostEnvironment,
            ServiceProvider,
            ConfiguratorLogger,
            swappableLoggerProvider);
    }

    public virtual void Dispose()
    {
        LoggerFactory?.Dispose();
        (ServiceProvider as IDisposable)?.Dispose();
    }

    private class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Test";
        public string ApplicationName { get; set; } = "TestApp";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = null!;
    }
}
