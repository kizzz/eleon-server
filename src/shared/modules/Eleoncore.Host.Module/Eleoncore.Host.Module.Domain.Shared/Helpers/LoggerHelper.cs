using Eleon.Logging.Lib.SystemLog.Contracts;
using Eleon.Logging.Lib.SystemLog.Enrichers;
using Eleon.Logging.Lib.SystemLog.Extensions;
using Eleon.Logging.Lib.SystemLog.Sinks;
using Microsoft.Extensions.Configuration;
using SharedModule.modules.Helpers.Module;
using SharedModule.modules.Logging.Module.SystemLog.Sinks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace EleonsoftSdk.modules.Helpers.Module;
public static class LoggerHelper
{
  public static void ConfigureDefaultEleoncoreLogger(this IConfiguration configuration)
  {
    var sinks = new List<ISystemLogSink>()
        {
            new ConsoleSystemLogSink().Filter(configuration, "Logger:Sinks:Console")
        };

    var allLogPath = BuildLogPath("All");
    var errorLogPath = BuildLogPath("Error");
    sinks.Add(new FileSystemLogSink(allLogPath).Filter(configuration, "Logger:Sinks:All", SystemLogLevel.Info));
    sinks.Add(new FileSystemLogSink(errorLogPath).Filter(configuration, "Logger:Sinks:Error", SystemLogLevel.Critical));

    sinks.Add(new ApiSystemLogSink().Filter(configuration, "Logger:Sinks:Api", SystemLogLevel.Critical));

    configuration.ConfigureEleonsoftLogger(sinks, [new HostInfoEnricher(), new HttpContextEnricher(), new CurrentTenantEnricher()]);
  }

  private static string BuildLogPath(string folder)
  {
    var fileName = $"systemlog-{DateTime.UtcNow:yyyyMMdd-HHmmss}.ndjson";
    return Path.Combine(AppContext.BaseDirectory, "Logs", folder, fileName);
  }
}
public class CurrentTenantEnricher : ISystemLogEnricher
{
  public void Enrich(Dictionary<string, string> entry)
  {
    var currentTenant = StaticServicesAccessor.GetService<ICurrentTenant>();

    var tenant = currentTenant?.Id?.ToString();

    if (string.IsNullOrEmpty(tenant))
    {
      tenant = "Root";
    }

    entry.AddIfNotExists("TenantId", tenant);
  }
}
