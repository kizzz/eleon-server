using SharedModule.modules.Otel.Module;

namespace Eleon.Telemetry.Lib.Domain.Tests.TestHelpers;

/// <summary>
/// Fluent builder for creating OtelOptions test scenarios.
/// </summary>
public class TelemetryTestDataBuilder
{
    private readonly OtelOptions _options;

    public TelemetryTestDataBuilder()
    {
        _options = new OtelOptions();
    }

    public TelemetryTestDataBuilder(OtelOptions options)
    {
        _options = options;
    }

    public TelemetryTestDataBuilder WithEnabled(bool enabled = true)
    {
        _options.Enabled = enabled;
        return this;
    }

    public TelemetryTestDataBuilder WithServiceName(string serviceName)
    {
        _options.ServiceName = serviceName;
        return this;
    }

    public TelemetryTestDataBuilder WithServiceDescription(string? description)
    {
        _options.ServiceDescription = description;
        return this;
    }

    public TelemetryTestDataBuilder WithTracesEndpoint(string endpoint)
    {
        _options.Traces.Endpoint = endpoint;
        return this;
    }

    public TelemetryTestDataBuilder WithTracesProtocol(string protocol)
    {
        _options.Traces.Protocol = protocol;
        return this;
    }

    public TelemetryTestDataBuilder WithTracesBatch(bool useBatch = true)
    {
        _options.Traces.UseBatch = useBatch;
        return this;
    }

    public TelemetryTestDataBuilder WithTracesInstrumentation(
        bool aspNetCore = true,
        bool httpClient = true,
        bool sqlClient = true,
        bool massTransit = true)
    {
        _options.Traces.UseAspNetCoreInstrumentation = aspNetCore;
        _options.Traces.UseHttpClientInstrumentation = httpClient;
        _options.Traces.UseSqlClientInstrumentation = sqlClient;
        _options.Traces.UseMassTransitInstrumentation = massTransit;
        return this;
    }

    public TelemetryTestDataBuilder WithMetricsEndpoint(string endpoint)
    {
        _options.Metrics.Endpoint = endpoint;
        return this;
    }

    public TelemetryTestDataBuilder WithMetricsProtocol(string protocol)
    {
        _options.Metrics.Protocol = protocol;
        return this;
    }

    public TelemetryTestDataBuilder WithMetricsBatch(bool useBatch = true)
    {
        _options.Metrics.UseBatch = useBatch;
        return this;
    }

    public TelemetryTestDataBuilder WithMetricsInstrumentation(
        bool runtime = true,
        bool process = true,
        bool aspNetCore = true,
        bool httpClient = true)
    {
        _options.Metrics.UseRuntimeInstrumentation = runtime;
        _options.Metrics.UseProcessInstrumentation = process;
        _options.Metrics.UseAspNetCoreInstrumentation = aspNetCore;
        _options.Metrics.UseHttpClientInstrumentation = httpClient;
        return this;
    }

    public TelemetryTestDataBuilder WithLogsEndpoint(string endpoint)
    {
        _options.Logs.Endpoint = endpoint;
        return this;
    }

    public TelemetryTestDataBuilder WithLogsProtocol(string protocol)
    {
        _options.Logs.Protocol = protocol;
        return this;
    }

    public TelemetryTestDataBuilder WithLogsBatch(bool useBatch = true)
    {
        _options.Logs.UseBatch = useBatch;
        return this;
    }

    public TelemetryTestDataBuilder WithLogsOptions(bool includeScopes = true, bool includeFormattedMessage = true)
    {
        _options.Logs.IncludeScopes = includeScopes;
        _options.Logs.IncludeFormattedMessage = includeFormattedMessage;
        return this;
    }

    public TelemetryTestDataBuilder WithInvalidTracesEndpoint()
    {
        _options.Traces.Endpoint = "";
        return this;
    }

    public TelemetryTestDataBuilder WithInvalidMetricsEndpoint()
    {
        _options.Metrics.Endpoint = "";
        return this;
    }

    public TelemetryTestDataBuilder WithInvalidLogsEndpoint()
    {
        _options.Logs.Endpoint = "";
        return this;
    }

    public TelemetryTestDataBuilder WithMalformedTracesEndpoint()
    {
        _options.Traces.Endpoint = "not-a-valid-uri";
        return this;
    }

    public TelemetryTestDataBuilder WithMalformedMetricsEndpoint()
    {
        _options.Metrics.Endpoint = "not-a-valid-uri";
        return this;
    }

    public TelemetryTestDataBuilder WithMalformedLogsEndpoint()
    {
        _options.Logs.Endpoint = "not-a-valid-uri";
        return this;
    }

    public OtelOptions Build()
    {
        return _options;
    }

    public static OtelOptions ValidOptions() => new TelemetryTestDataBuilder()
        .WithEnabled()
        .WithServiceName("TestService")
        .WithTracesEndpoint("http://localhost:4318/v1/traces")
        .WithMetricsEndpoint("http://localhost:4318/v1/metrics")
        .WithLogsEndpoint("http://localhost:4318/v1/logs")
        .Build();

    public static OtelOptions DisabledOptions() => new TelemetryTestDataBuilder()
        .WithEnabled(false)
        .Build();

    public static OtelOptions InvalidOptions() => new TelemetryTestDataBuilder()
        .WithEnabled()
        .WithInvalidTracesEndpoint()
        .WithInvalidMetricsEndpoint()
        .WithInvalidLogsEndpoint()
        .Build();
}
