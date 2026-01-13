using Eleon.Logging.Lib.SystemLog.Contracts;
using EleonsoftModuleCollector.Commons.Module.Constants;
using SharedModule.modules.Otel.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
public class TenantSystemHealthSettings
{
  public TelemetrySettings Telemetry { get; set; } = new TelemetrySettings();
  public LoggingSettings Logging { get; set; } = new LoggingSettings();
}

public class LoggingSettings
{
  public bool SendLogsFromUI { get; set; } = false;
  public SystemLogLevel MinimumLogLevel { get; set; } = SystemLogLevel.Warning;
}

public class TelemetrySettings
{
  public bool Enabled { get; set; } = false;
  public bool EnabledOnClient { get; set; } = false;

  public string TracesEndpoint { get; set; } = "http://localhost:4318/v1/traces";
  public string TracesProtocol { get; set; } = "http"; // "grpc" | "http"
  public bool TracesUseBatch { get; set; } = true;

  public string MetricsEndpoint { get; set; } = "http://localhost:4318/v1/metrics";
  public string MetricsProtocol { get; set; } = "http"; // "grpc" | "http"
  public bool MetricsUseBatch { get; set; } = true;

  public string LogsEndpoint { get; set; } = "http://localhost:4318/v1/logs";
  public string LogsProtocol { get; set; } = "http"; // "grpc" | "http"
  public bool LogsUseBatch { get; set; } = true;
  public Guid StorageProviderId { get; set; }

  public OtelOptions ToOptions()
  {
    return new OtelOptions
    {
      Enabled = this.Enabled,
      Traces = new OtelOptions.TracesOptions
      {
        Endpoint = this.TracesEndpoint,
        Protocol = this.TracesProtocol,
        UseBatch = this.TracesUseBatch,
        UseAspNetCoreInstrumentation = true,
        UseHttpClientInstrumentation = true,
        UseSqlClientInstrumentation = true,
        UseMassTransitInstrumentation = true,
      },
      Metrics = new OtelOptions.MetricsOptions
      {
        Endpoint = this.MetricsEndpoint,
        Protocol = this.MetricsProtocol,
        UseBatch = this.MetricsUseBatch,
        UseRuntimeInstrumentation = true,
        UseProcessInstrumentation = true,
        UseAspNetCoreInstrumentation = true,
        UseHttpClientInstrumentation = true,
      },
      Logs = new OtelOptions.LogsOptions
      {
        Endpoint = this.LogsEndpoint,
        Protocol = this.LogsProtocol,
        UseBatch = this.LogsUseBatch,
        IncludeScopes = true,
        IncludeFormattedMessage = true,
      }
    };
  }
}
