using Eleon.Logging.Lib.SystemLog.Contracts;
using EleonsoftModuleCollector.Commons.Module.Constants;
using EleonsoftModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using SharedModule.modules.Otel.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.TenantSettings;

public class TenantSystemHealthSettingsDto
{
  public TelemetrySettingsDto Telemetry { get; set; }
  public LoggingSettingsDto Logging { get; set; }
}

public class LoggingSettingsDto
{
  public bool SendLogsFromUI { get; set; } = true;
  public SystemLogLevel MinimumLogLevel { get; set; } = SystemLogLevel.Warning;
}

public class TelemetrySettingsDto
{
  public bool Enabled { get; set; } = false;
  public bool EnabledOnClient { get; set; } = true;

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
}
