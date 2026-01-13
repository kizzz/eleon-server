using MassTransit.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SharedModule.modules.Helpers.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReliabilityOptions = SharedModule.modules.Otel.Module.OtelOptions.ReliabilityOptions;

namespace SharedModule.modules.Otel.Module
{
  public sealed class TelemetryConfigurator : IDisposable
  {
    private readonly object _gate = new();
    private readonly SemaphoreSlim _configureLock = new(1, 1);
    private TracerProvider? _tracerProvider;
    private MeterProvider? _meterProvider;
    private SwappableLoggerProvider? _swappableLoggerProvider;
    private Func<HttpClient>? _httpClientFactory;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IHostEnvironment _env;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelemetryConfigurator> _logger;
    private readonly OtelOptions _configOptions;
    private OtelOptions _prevOptions;
    private readonly TelemetrySelfMetrics? _selfMetrics;
    private readonly TelemetryCircuitBreaker? _circuitBreaker;

    public TelemetryConfigurator(
        ILoggerFactory loggerFactory,
        IOptions<OtelOptions> options,
        IHostEnvironment hostEnvironment,
        IServiceProvider serviceProvider,
        ILogger<TelemetryConfigurator> logger,
        SwappableLoggerProvider? swappableLoggerProvider = null,
        TelemetrySelfMetrics? selfMetrics = null,
        TelemetryCircuitBreaker? circuitBreaker = null)
    {
      _loggerFactory = loggerFactory;
      _env = hostEnvironment;
      _serviceProvider = serviceProvider;
      _logger = logger;
      _configOptions = options.Value;
      _prevOptions = _configOptions;
      _swappableLoggerProvider = swappableLoggerProvider;
      _selfMetrics = selfMetrics;
      _circuitBreaker = circuitBreaker;
    }

    public void SetHttpClientFactory(Func<HttpClient> factory)
    {
      _httpClientFactory = factory;
    }

    public async Task ConfigureAsync(OtelOptions otel, bool force = false)
    {
      if (otel == null)
      {
        return;
      }

      _selfMetrics?.IncrementConfigureAttempts();

      await _configureLock.WaitAsync();
      try
      {
        // Check if options changed (inside lock to ensure thread-safety)
        if (_prevOptions.Equals(otel) && !force)
        {
          return;
        }

        _prevOptions = otel;

        // If disabled — tear everything down and return
        if (!(otel?.Enabled ?? false))
        {
          _selfMetrics?.SetEnabledState("traces", false);
          _selfMetrics?.SetEnabledState("metrics", false);
          _selfMetrics?.SetEnabledState("logs", false);
          SwapProviders(newTracer: null, newMeter: null, newLogProvider: null);
          return;
        }

        // Validate signals before building providers
        var (tracesValid, metricsValid, logsValid) = ValidateSignals(otel);

        // Update enabled state metrics
        _selfMetrics?.SetEnabledState("traces", tracesValid);
        _selfMetrics?.SetEnabledState("metrics", metricsValid);
        _selfMetrics?.SetEnabledState("logs", logsValid);

        if (!tracesValid && !metricsValid && !logsValid)
        {
          _logger.LogWarning("All telemetry signals are invalid. Tearing down providers.");
          _selfMetrics?.IncrementConfigureFailures();
          SwapProviders(newTracer: null, newMeter: null, newLogProvider: null);
          return;
        }

        // ---- Build TracerProvider (only if valid)
        TracerProvider? newTracer = null;
        if (tracesValid)
        {
          var tracerBuilder = Sdk.CreateTracerProviderBuilder()
              .SetResourceBuilder(GetResource(otel));

          if (_configOptions.Traces.UseAspNetCoreInstrumentation)
            tracerBuilder.AddAspNetCoreInstrumentation(o => { o.RecordException = true; });

          if (_configOptions.Traces.UseHttpClientInstrumentation)
            tracerBuilder.AddHttpClientInstrumentation(o => o.RecordException = true);

          if (_configOptions.Traces.UseSqlClientInstrumentation)
            tracerBuilder.AddSqlClientInstrumentation(o =>
            {
              o.SetDbStatementForText = true;
              o.RecordException = true;
            });

          if (_configOptions.Traces.UseMassTransitInstrumentation)
            tracerBuilder.AddSource(DiagnosticHeaders.DefaultListenerName);

          tracerBuilder.AddOtlpExporter(o =>
          {
            o.Endpoint = new Uri(otel.Traces.Endpoint);
            o.Protocol = ResolveProtocol(otel.Traces.Protocol);
            o.ExportProcessorType = otel.Traces.UseBatch ? ExportProcessorType.Batch : ExportProcessorType.Simple;
            if (_httpClientFactory != null) o.HttpClientFactory = _httpClientFactory;

            // Apply headers from reliability options
            var headers = OtlpHeadersResolver.ResolveHeaders(GetEffectiveReliabilityOptions(otel, "traces")?.Headers, _logger);
            if (headers != null)
            {
              o.Headers = string.Join(",", headers.Select(kvp => $"{kvp.Key}={kvp.Value}"));
              OtlpHeadersResolver.LogHeaderKeys(headers, _logger, "Traces exporter");
            }
          });

          newTracer = tracerBuilder.Build();
        }

        // ---- Build MeterProvider (only if valid)
        MeterProvider? newMeter = null;
        if (metricsValid)
        {
          var meterBuilder = Sdk.CreateMeterProviderBuilder()
              .SetResourceBuilder(GetResource(otel));

          if (_configOptions.Metrics.UseAspNetCoreInstrumentation)
            meterBuilder.AddAspNetCoreInstrumentation();

          if (_configOptions.Metrics.UseHttpClientInstrumentation)
            meterBuilder.AddHttpClientInstrumentation();

          if (_configOptions.Metrics.UseRuntimeInstrumentation)
            meterBuilder.AddRuntimeInstrumentation();

          if (_configOptions.Metrics.UseProcessInstrumentation)
            meterBuilder.AddProcessInstrumentation();

          meterBuilder.AddOtlpExporter(o =>
          {
            o.Endpoint = new Uri(otel.Metrics.Endpoint);
            o.Protocol = ResolveProtocol(otel.Metrics.Protocol);
            o.ExportProcessorType = otel.Metrics.UseBatch ? ExportProcessorType.Batch : ExportProcessorType.Simple;
            if (_httpClientFactory != null) o.HttpClientFactory = _httpClientFactory;

            // Apply headers from reliability options
            var headers = OtlpHeadersResolver.ResolveHeaders(GetEffectiveReliabilityOptions(otel, "metrics")?.Headers, _logger);
            if (headers != null)
            {
              o.Headers = string.Join(",", headers.Select(kvp => $"{kvp.Key}={kvp.Value}"));
              OtlpHeadersResolver.LogHeaderKeys(headers, _logger, "Metrics exporter");
            }
          });

          newMeter = meterBuilder.Build();
        }

        // ---- Build OpenTelemetryLoggerProvider (only if valid)
        OpenTelemetryLoggerProvider? newLogProvider = null;
        if (logsValid)
        {
          var logOptions = new OpenTelemetryLoggerOptions
          {
            IncludeScopes = _configOptions.Logs.IncludeScopes,
            IncludeFormattedMessage = _configOptions.Logs.IncludeFormattedMessage,
          }
              .SetResourceBuilder(GetResource(otel))
              .AddOtlpExporter(exp =>
              {
                exp.Endpoint = new Uri(otel.Logs.Endpoint);
                exp.Protocol = ResolveProtocol(otel.Logs.Protocol);
                exp.ExportProcessorType = otel.Logs.UseBatch ? ExportProcessorType.Batch : ExportProcessorType.Simple;
                if (_httpClientFactory != null) exp.HttpClientFactory = _httpClientFactory;

                // Apply headers from reliability options
                var headers = OtlpHeadersResolver.ResolveHeaders(GetEffectiveReliabilityOptions(otel, "logs")?.Headers, _logger);
                if (headers != null)
                {
                  exp.Headers = string.Join(",", headers.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                  OtlpHeadersResolver.LogHeaderKeys(headers, _logger, "Logs exporter");
                }
              });

          newLogProvider = new OpenTelemetryLoggerProvider(new FixedOptionsMonitor<OpenTelemetryLoggerOptions>(logOptions));
        }

        // ---- Atomically swap providers (dispose old in background after flush)
        SwapProviders(newTracer, newMeter, newLogProvider);

        // Record successful configuration
        var unixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        _selfMetrics?.SetLastSuccessTimestamp(unixSeconds);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error during telemetry configuration");
        _selfMetrics?.IncrementConfigureFailures();
        _selfMetrics?.SetEnabledState("traces", false);
        _selfMetrics?.SetEnabledState("metrics", false);
        _selfMetrics?.SetEnabledState("logs", false);
        throw;
      }
      finally
      {
        _configureLock.Release();
      }
    }

    public void Dispose()
    {
      lock (_gate)
      {
        _swappableLoggerProvider?.Swap(null);
        _tracerProvider?.Dispose();
        _meterProvider?.Dispose();
        _tracerProvider = null;
        _meterProvider = null;
      }
      _configureLock?.Dispose();
    }

    // ---------- helpers ----------
    private void SwapProviders(TracerProvider? newTracer, MeterProvider? newMeter, OpenTelemetryLoggerProvider? newLogProvider)
    {
      TracerProvider? oldTracer;
      MeterProvider? oldMeter;

      lock (_gate)
      {
        oldTracer = _tracerProvider;
        oldMeter = _meterProvider;

        _tracerProvider = newTracer;
        _meterProvider = newMeter;

        // Swap logger provider via SwappableLoggerProvider instead of AddProvider
        if (_swappableLoggerProvider != null)
        {
          _swappableLoggerProvider.Swap(newLogProvider);
        }
      }

      // Flush/Dispose old ones promptly to avoid memory buildup on frequent reconfigures
      try { oldTracer?.ForceFlush(10_000); } catch { }
      oldTracer?.Dispose();

      try { oldMeter?.ForceFlush(10_000); } catch { }
      oldMeter?.Dispose();
    }

    private (bool tracesValid, bool metricsValid, bool logsValid) ValidateSignals(OtelOptions options)
    {
      bool tracesValid = !string.IsNullOrWhiteSpace(options.Traces.Endpoint) &&
                         Uri.TryCreate(options.Traces.Endpoint, UriKind.Absolute, out _);
      bool metricsValid = !string.IsNullOrWhiteSpace(options.Metrics.Endpoint) &&
                          Uri.TryCreate(options.Metrics.Endpoint, UriKind.Absolute, out _);
      bool logsValid = !string.IsNullOrWhiteSpace(options.Logs.Endpoint) &&
                       Uri.TryCreate(options.Logs.Endpoint, UriKind.Absolute, out _);

      // Validate TLS settings: AllowInvalidCertificate only allowed in dev
      var tracesReliability = GetEffectiveReliabilityOptions(options, "traces");
      if (tracesReliability?.Tls?.AllowInvalidCertificate == true && !_env.IsDevelopment())
      {
        _logger.LogWarning("AllowInvalidCertificate TLS setting is only allowed in Development environment. Traces will be disabled.");
        tracesValid = false;
        _selfMetrics?.IncrementConfigureFailures();
      }

      var metricsReliability = GetEffectiveReliabilityOptions(options, "metrics");
      if (metricsReliability?.Tls?.AllowInvalidCertificate == true && !_env.IsDevelopment())
      {
        _logger.LogWarning("AllowInvalidCertificate TLS setting is only allowed in Development environment. Metrics will be disabled.");
        metricsValid = false;
        _selfMetrics?.IncrementConfigureFailures();
      }

      var logsReliability = GetEffectiveReliabilityOptions(options, "logs");
      if (logsReliability?.Tls?.AllowInvalidCertificate == true && !_env.IsDevelopment())
      {
        _logger.LogWarning("AllowInvalidCertificate TLS setting is only allowed in Development environment. Logs will be disabled.");
        logsValid = false;
        _selfMetrics?.IncrementConfigureFailures();
      }

      if (!tracesValid)
        _logger.LogWarning("Invalid traces endpoint: {Endpoint}. Traces will be disabled.", options.Traces.Endpoint);
      if (!metricsValid)
        _logger.LogWarning("Invalid metrics endpoint: {Endpoint}. Metrics will be disabled.", options.Metrics.Endpoint);
      if (!logsValid)
        _logger.LogWarning("Invalid logs endpoint: {Endpoint}. Logs will be disabled.", options.Logs.Endpoint);

      return (tracesValid, metricsValid, logsValid);
    }

    private ReliabilityOptions? GetEffectiveReliabilityOptions(OtelOptions options, string signalType)
    {
      ReliabilityOptions? signalReliability = null;
      switch (signalType.ToLowerInvariant())
      {
        case "traces":
          signalReliability = options.Traces.Reliability;
          break;
        case "metrics":
          signalReliability = options.Metrics.Reliability;
          break;
        case "logs":
          signalReliability = options.Logs.Reliability;
          break;
      }

      // Merge: per-signal override takes precedence, fall back to global
      if (signalReliability != null)
        return signalReliability;
      return options.Reliability;
    }

    private static OtlpExportProtocol ResolveProtocol(string? protocol) =>
        (protocol ?? "").Trim().ToLowerInvariant() switch
        {
          "grpc" => OtlpExportProtocol.Grpc,
          "http" => OtlpExportProtocol.HttpProtobuf,
          _ => OtlpExportProtocol.Grpc
        };

    // Minimal IOptionsMonitor implementation to new-up OpenTelemetryLoggerProvider
    private sealed class FixedOptionsMonitor<T> : IOptionsMonitor<T>
    {
      private readonly T _value;
      public FixedOptionsMonitor(T value) => _value = value;
      public T CurrentValue => _value;
      public T Get(string? name) => _value;
      public IDisposable OnChange(Action<T, string?> listener) => EmptyDisposable.Instance;

      private sealed class EmptyDisposable : IDisposable
      {
        public static readonly EmptyDisposable Instance = new();
        public void Dispose() { }
      }
    }

    /// <summary>
    /// Builds OpenTelemetry resource attributes.
    /// 
    /// Attribute guidance:
    /// - Use bounded, stable attributes (service.name, deployment.environment, host.name)
    /// - Avoid high-cardinality attributes (no tenant domain lists, user IDs, query strings)
    /// - Avoid sensitive data (no tenant domains, no personal identifiers)
    /// - tenant.id can be added if telemetry backend supports it (currently not included)
    /// - Optional bounded attributes: tenant.tier, deployment.ring (not currently included)
    /// </summary>
    private ResourceBuilder GetResource(OtelOptions? tenantOptions)
    {
      var builder = ResourceBuilder.CreateDefault()
              .AddService(_configOptions?.ServiceName ?? tenantOptions?.ServiceName ?? "undefined", serviceVersion: VersionHelper.Version)
              .AddAttributes(new[]
              {
                        new KeyValuePair<string, object>("deployment.environment", _env.EnvironmentName ?? "Production"),
                        new KeyValuePair<string, object>("host.name", Environment.MachineName),
                        new KeyValuePair<string, object>("service.description", tenantOptions?.ServiceDescription ?? ""),
                        new KeyValuePair<string, object>("host.session.id", SessionId),
              });

      return builder;
    }

    private static readonly string SessionId = Guid.NewGuid().ToString();
  }
}
