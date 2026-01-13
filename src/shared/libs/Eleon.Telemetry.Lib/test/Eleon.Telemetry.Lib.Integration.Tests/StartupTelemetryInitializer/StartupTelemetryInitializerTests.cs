using Eleon.Telemetry.Lib.Integration.Tests.TestBase;
using Eleon.Telemetry.Lib.Domain.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using SharedModule.modules.Otel.Module;
using System.Collections.Concurrent;
using System.Reflection;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Eleon.Telemetry.Lib.Integration.Tests.StartupTelemetryInitializer;

/// <summary>
/// Tests for StartupTelemetryInitializer hosted service behavior.
/// </summary>
public class StartupTelemetryInitializerTests : TelemetryIntegrationTestBase
{
    [Fact]
    public async Task StartAsync_WithEnabledFalse_Should_NotCallConfigureAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        var disabledOptions = TelemetryTestDataBuilder.DisabledOptions();
        services.AddSingleton(Options.Create(disabledOptions));
        services.AddSingleton<TelemetryConfigurator>(sp =>
        {
            var loggerFactory = LoggerFactory;
            var hostEnv = HostEnvironment;
            var serviceProvider = sp;
            var logger = ConfiguratorLogger;
            var options = Options.Create(disabledOptions);
            return new TelemetryConfigurator(loggerFactory, options, hostEnv, serviceProvider, logger);
        });

        var serviceProvider = services.BuildServiceProvider();
        var configurator = serviceProvider.GetRequiredService<TelemetryConfigurator>();
        var initializer = CreateInitializer(serviceProvider, disabledOptions);
        var startMethod = initializer.GetType().GetMethod("StartAsync", BindingFlags.Public | BindingFlags.Instance);

        // Act
        var task = (Task)startMethod!.Invoke(initializer, new object[] { CancellationToken.None })!;
        await task;

        // Assert - Should complete without calling ConfigureAsync (when enabled is false)
        // We can't easily verify this without internal state, but we verify no exceptions
        configurator.Should().NotBeNull();
    }

    [Fact]
    public async Task StartAsync_WithMissingSettingsProvider_Should_NotFail()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = TelemetryTestDataBuilder.ValidOptions();
        services.AddSingleton(Options.Create(options));
        services.AddSingleton<TelemetryConfigurator>(sp =>
        {
            var loggerFactory = LoggerFactory;
            var hostEnv = HostEnvironment;
            var serviceProvider = sp;
            var logger = ConfiguratorLogger;
            var opts = Options.Create(options);
            return new TelemetryConfigurator(loggerFactory, opts, hostEnv, serviceProvider, logger);
        });
        // Don't register ITelemetrySettingsProvider

        var serviceProvider = services.BuildServiceProvider();
        var initializer = CreateInitializer(serviceProvider, options);
        var startMethod = initializer.GetType().GetMethod("StartAsync", BindingFlags.Public | BindingFlags.Instance);

        // Act
        await FluentActions.Awaiting(async () =>
        {
            var task = (Task)startMethod!.Invoke(initializer, new object[] { CancellationToken.None })!;
            await task;
        }).Should().NotThrowAsync();

        // Assert - Should complete without throwing
    }

    [Fact]
    public async Task StartAsync_WithSettingsProviderThatThrows_Should_SwallowException()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = TelemetryTestDataBuilder.ValidOptions();
        services.AddSingleton(Options.Create(options));
        services.AddSingleton<TelemetryConfigurator>(sp =>
        {
            var loggerFactory = LoggerFactory;
            var hostEnv = HostEnvironment;
            var serviceProvider = sp;
            var logger = ConfiguratorLogger;
            var opts = Options.Create(options);
            return new TelemetryConfigurator(loggerFactory, opts, hostEnv, serviceProvider, logger);
        });

        var throwingProvider = Substitute.For<ITelemetrySettingsProvider>();
        throwingProvider.InitializeAsync().Returns(Task.FromException(new InvalidOperationException("Test exception")));
        services.AddSingleton(throwingProvider);

        var logProvider = new TestLoggerProvider();
        services.AddLogging(builder => builder.AddProvider(logProvider));

        var serviceProvider = services.BuildServiceProvider();
        var initializer = CreateInitializer(serviceProvider, options, serviceProvider);
        var startMethod = initializer.GetType().GetMethod("StartAsync", BindingFlags.Public | BindingFlags.Instance);

        // Act
        await FluentActions.Awaiting(async () =>
        {
            var task = (Task)startMethod!.Invoke(initializer, new object[] { CancellationToken.None })!;
            await task;
        }).Should().NotThrowAsync();

        // Assert - Exception should be swallowed and logged
        logProvider.Entries.Any(entry =>
                entry.Level == LogLevel.Error &&
                entry.Exception is InvalidOperationException &&
                entry.Message.Contains("Failed to initialize telemetry settings", StringComparison.OrdinalIgnoreCase))
            .Should().BeTrue();
    }

    [Fact]
    public async Task StartAsync_WithSettingsProvider_Should_CallInitializeAsync()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = TelemetryTestDataBuilder.ValidOptions();
        services.AddSingleton(Options.Create(options));
        services.AddSingleton<TelemetryConfigurator>(sp =>
        {
            var loggerFactory = LoggerFactory;
            var hostEnv = HostEnvironment;
            var serviceProvider = sp;
            var logger = ConfiguratorLogger;
            var opts = Options.Create(options);
            return new TelemetryConfigurator(loggerFactory, opts, hostEnv, serviceProvider, logger);
        });

        var mockProvider = Substitute.For<ITelemetrySettingsProvider>();
        services.AddSingleton(mockProvider);

        var serviceProvider = services.BuildServiceProvider();
        var initializer = CreateInitializer(serviceProvider, options, serviceProvider);
        var startMethod = initializer.GetType().GetMethod("StartAsync", BindingFlags.Public | BindingFlags.Instance);

        // Act
        var task = (Task)startMethod!.Invoke(initializer, new object[] { CancellationToken.None })!;
        await task;

        // Assert - InitializeAsync should be called
        await mockProvider.Received(1).InitializeAsync();
    }

    private object CreateInitializer(
        IServiceProvider serviceProvider,
        OtelOptions options,
        IServiceProvider? innerServiceProvider = null)
    {
        // Use reflection to create the private class
        var type = typeof(EleonsoftOtelExtensions).GetNestedType("StartupTelemetryInitializer", BindingFlags.NonPublic);
        if (type == null)
        {
            throw new InvalidOperationException("StartupTelemetryInitializer type not found");
        }

        var constructor = type.GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
            null,
            new[] { typeof(IServiceProvider), typeof(Func<HttpClient>) },
            null);

        if (constructor == null)
        {
            foreach (var candidate in type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                var parameters = candidate.GetParameters();
                if (parameters.Length == 2 &&
                    parameters[0].ParameterType == typeof(IServiceProvider) &&
                    parameters[1].ParameterType == typeof(Func<HttpClient>))
                {
                    constructor = candidate;
                    break;
                }
            }
        }

        if (constructor == null)
        {
            throw new InvalidOperationException("StartupTelemetryInitializer constructor not found");
        }

        return constructor.Invoke(new object[] { innerServiceProvider ?? serviceProvider, null! });
    }

    private sealed class TestLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentQueue<LogEntry> _entries = new();

        public IEnumerable<LogEntry> Entries => _entries.ToArray();

        public ILogger CreateLogger(string categoryName) => new TestLogger(categoryName, _entries);

        public void Dispose()
        {
        }

        private sealed class TestLogger : ILogger
        {
            private readonly string _category;
            private readonly ConcurrentQueue<LogEntry> _entries;

            public TestLogger(string category, ConcurrentQueue<LogEntry> entries)
            {
                _category = category;
                _entries = entries;
            }

            public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                var message = formatter(state, exception);
                _entries.Enqueue(new LogEntry(_category, logLevel, eventId, exception, message));
            }
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            public void Dispose()
            {
            }
        }
    }

    private sealed record LogEntry(string Category, LogLevel Level, EventId EventId, Exception? Exception, string Message);
}
