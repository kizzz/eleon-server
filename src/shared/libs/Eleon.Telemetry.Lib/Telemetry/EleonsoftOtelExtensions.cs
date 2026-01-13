using Eleon.Logging.Lib.VportalLogging;
using MassTransit.Configuration;
using MassTransit.Logging;
using Microsoft.Extensions.Configuration;
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
using SharedModule.HttpApi.Helpers;
using SharedModule.modules.Helpers.Module;
using System;

namespace SharedModule.modules.Otel.Module;
public static class EleonsoftOtelExtensions
{
  public static IHostBuilder UseEleonsoftOtel(
  this IHostBuilder builder,
  OtelOptions? otelOptions = null,
  Func<HttpClient>? httpClientFactory = null)
  {
    builder.ConfigureServices((context, services) =>
    {
      otelOptions ??= new OtelOptions();

      context.Configuration.GetSection("Telemetry").Bind(otelOptions);
      services.Configure<OtelOptions>(context.Configuration.GetSection("Telemetry"));
      services.Configure<OtelOptions>(opt =>
          {
          if (string.IsNullOrEmpty(opt.ServiceName))
          {
            opt.ServiceName = context.Configuration.GetValue<string>("ApplicationName") ?? "undefined";
          }
        });

      // Register TelemetrySelfMetrics as singleton
      services.AddSingleton<TelemetrySelfMetrics>();

      // Register TelemetryCircuitBreaker as singleton
      services.AddSingleton<TelemetryCircuitBreaker>();

      // Register SwappableLoggerProvider once to prevent accumulation
      services.AddSingleton<SwappableLoggerProvider>(sp =>
      {
        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
        var swappableProvider = new SwappableLoggerProvider();
        loggerFactory.AddProvider(swappableProvider);
        return swappableProvider;
      });

      services.AddSingleton<TelemetryConfigurator>(sp =>
      {
        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
        var options = sp.GetRequiredService<IOptions<OtelOptions>>();
        var hostEnvironment = sp.GetRequiredService<IHostEnvironment>();
        var logger = sp.GetRequiredService<ILogger<TelemetryConfigurator>>();
        var swappableProvider = sp.GetRequiredService<SwappableLoggerProvider>();
        var selfMetrics = sp.GetRequiredService<TelemetrySelfMetrics>();
        var circuitBreaker = sp.GetRequiredService<TelemetryCircuitBreaker>();
        return new TelemetryConfigurator(loggerFactory, options, hostEnvironment, sp, logger, swappableProvider, selfMetrics, circuitBreaker);
      });

      services.AddHostedService(sp => new StartupTelemetryInitializer(sp, httpClientFactory));
    });

    return builder;
  }

  private sealed class StartupTelemetryInitializer : IHostedService
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly TelemetryConfigurator _tm;
    private readonly OtelOptions _opts;
    private readonly Func<HttpClient>? _httpClientFactory;

    public StartupTelemetryInitializer(
        IServiceProvider serviceProvider,
        Func<HttpClient>? httpClientFactory)
    {
      _serviceProvider = serviceProvider;
      _tm = serviceProvider.GetRequiredService<TelemetryConfigurator>();
      _opts = serviceProvider.GetRequiredService<IOptions<OtelOptions>>().Value;
      _httpClientFactory = httpClientFactory;
    }

    public async Task StartAsync(CancellationToken ct)
    {
      var boundaryLogger = _serviceProvider.GetService<IBoundaryLogger>();
      using var _ = boundaryLogger?.Begin("HostedService StartupTelemetryInitializer");
      if (_httpClientFactory != null)
      {
        _tm.SetHttpClientFactory(_httpClientFactory);
      }

      if (_opts.Enabled)
      {
        await _tm.ConfigureAsync(_opts, true);
      }

      try
      {
        using var scope = _serviceProvider.CreateScope();

        var settingProvider = scope.ServiceProvider.GetService<ITelemetrySettingsProvider>();

        if (settingProvider != null)
        {
          await settingProvider.InitializeAsync().ConfigureAwait(false);
        }
      }
      catch (Exception ex)
      {
        _serviceProvider.GetService<ILogger<StartupTelemetryInitializer>>()?.LogError(ex, "Failed to initialize telemetry settings");
      }
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
  }

  [Obsolete("This otel is not reconfigurable. Use UseEleonsoftOtel instead")]
  public static IHostBuilder UseEleonsoftOtelStatic(
      this IHostBuilder builder,
      OtelOptions? otelOptions = null,
      Func<HttpClient>? httpClientFactory = null)
  {
    builder.ConfigureServices((context, services) =>
    {
      var configuration = context.Configuration;
      var env = context.HostingEnvironment;

      // Load from DI/config if not provided
      otelOptions ??= new OtelOptions();

      configuration.GetSection("Telemetry").Bind(otelOptions);

      if (!otelOptions.Enabled)
        return;

      // Resolve service identity
      var serviceName = string.IsNullOrEmpty(otelOptions.ServiceName) ? configuration.GetValue<string>("ApplicationName") ?? "undefined" : otelOptions.ServiceName;

      var serviceVersion = VersionHelper.Version;

      services.AddOpenTelemetry()
          .ConfigureResource(r => r
              .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
              .AddAttributes(new[]
              {
                    new KeyValuePair<string, object>("deployment.environment", env.EnvironmentName),
                    new KeyValuePair<string, object>("host.name", Environment.MachineName),
                    new KeyValuePair<string, object>("service.description", otelOptions.ServiceDescription ?? ""),
                    new KeyValuePair<string, object>("host.session.id", Guid.NewGuid().ToString()),
            }))
          .WithTracing(tracer =>
          {
          if (otelOptions.Traces.UseAspNetCoreInstrumentation)
          {
            tracer.AddAspNetCoreInstrumentation(o =>
                {
                  o.RecordException = true;
                  // Light enrichment example:
                  o.EnrichWithHttpRequest = (act, req) =>
                      {
                      if (req.ContentLength is not null)
                        act.SetTag("http.request_content_length", req.ContentLength);
                    };
                });
          }

          if (otelOptions.Traces.UseHttpClientInstrumentation)
            tracer.AddHttpClientInstrumentation(o => o.RecordException = true);

          if (otelOptions.Traces.UseSqlClientInstrumentation)
            tracer.AddSqlClientInstrumentation(o =>
                {
                  o.SetDbStatementForText = true;
                  o.RecordException = true;
                });

          if (otelOptions.Traces.UseMassTransitInstrumentation)
          {
            tracer.AddSource(DiagnosticHeaders.DefaultListenerName);
          }

          tracer.AddOtlpExporter(options =>
              {
              options.Endpoint = new Uri(otelOptions.Traces.Endpoint);
              options.Protocol = ResolveProtocol(otelOptions.Traces.Protocol);
              options.ExportProcessorType = otelOptions.Traces.UseBatch ? ExportProcessorType.Batch : ExportProcessorType.Simple;
              if (httpClientFactory != null) options.HttpClientFactory = httpClientFactory;
            });
        })
          .WithMetrics(meter =>
          {
          if (otelOptions.Metrics.UseAspNetCoreInstrumentation)
            meter.AddAspNetCoreInstrumentation();

          if (otelOptions.Metrics.UseHttpClientInstrumentation)
            meter.AddHttpClientInstrumentation();

          if (otelOptions.Metrics.UseRuntimeInstrumentation)
            meter.AddRuntimeInstrumentation();

          if (otelOptions.Metrics.UseProcessInstrumentation)
            meter.AddProcessInstrumentation();

          meter.AddOtlpExporter(options =>
              {
              options.Endpoint = new Uri(otelOptions.Metrics.Endpoint);
              options.Protocol = ResolveProtocol(otelOptions.Metrics.Protocol);
              options.ExportProcessorType = otelOptions.Metrics.UseBatch ? ExportProcessorType.Batch : ExportProcessorType.Simple;
              if (httpClientFactory != null) options.HttpClientFactory = httpClientFactory;
            });
        })
          .WithLogging(logging =>
          {
          logging.AddOtlpExporter(options =>
              {
              options.Endpoint = new Uri(otelOptions.Logs.Endpoint);
              options.Protocol = ResolveProtocol(otelOptions.Logs.Protocol);
              options.ExportProcessorType = otelOptions.Logs.UseBatch ? ExportProcessorType.Batch : ExportProcessorType.Simple;
              if (httpClientFactory != null) options.HttpClientFactory = httpClientFactory;

            });
        }, loggerOptions =>
        {
          loggerOptions.IncludeScopes = otelOptions.Logs.IncludeScopes;
          loggerOptions.IncludeFormattedMessage = otelOptions.Logs.IncludeFormattedMessage;
        });
    });

    return builder;
  }

  public static Func<HttpClient> GetDefaultHttpClientFactory(IConfiguration configuration)
  {
    return () => SdkHttpClientFactory.Create(new ApiConfigurator(new EleoncoreSdkConfig().FromConfiguration(configuration)), "api", "OtelHttpClient");
  }

  private static OtlpExportProtocol ResolveProtocol(string protocol)
  {
    return protocol.Trim().ToLowerInvariant() switch
    {
      "grpc" => OtlpExportProtocol.Grpc,
      "http" => OtlpExportProtocol.HttpProtobuf,
      _ => OtlpExportProtocol.Grpc,
    };
  }
}
