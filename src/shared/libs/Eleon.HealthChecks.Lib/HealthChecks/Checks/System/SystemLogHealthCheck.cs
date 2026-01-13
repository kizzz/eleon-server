using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using HealthCheckModule.Module.Domain.Shared.Constants;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Eleon.Common.Lib.modules.HealthCheck.Module.General;
using Eleon.Logging.Lib.SystemLog.Logger;
using Eleon.Logging.Lib.SystemLog.Sinks;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.System;

public class SystemLogHealthCheck : DefaultHealthCheck
{
  public SystemLogHealthCheck(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  public override string Name => "SystemLog";
  public override bool IsPublic => true;

  public override async Task ExecuteCheckAsync(HealthCheckReportEto report)
  {
    report.Status = HealthCheckStatus.OK;
    report.Message = "No queued log entries were detected.";

    var sinks = EleonsoftLog.GetSinksSnapshot() ?? Array.Empty<object>();

    int totalSinks = 0;
    int errors = 0;
    int totalFlushed = 0;

    foreach (var sink in sinks)
    {
      var sinkType = sink?.GetType().FullName ?? "(null)";

      try
      {
        if (sink is AbstractQueuedSystemLogSink queuedSink1 || (sink is FilteringSink fSink && fSink.Sink is AbstractQueuedSystemLogSink queuedSink2))
        {
          totalSinks++;

          // TryFlushNowAsync is assumed to return KeyValuePair<int, IReadOnlyList<SystemLogEntry>>
          AbstractQueuedSystemLogSink queuedSink;

          if (sink is AbstractQueuedSystemLogSink qs1)
            queuedSink = qs1;
          else if (sink is FilteringSink fs && fs.Sink is AbstractQueuedSystemLogSink qs2)
          {
            queuedSink = qs2;
            sinkType = queuedSink.GetType().FullName ?? "(null)";
          }
          else
            throw new InvalidOperationException("Unexpected sink type.");

          var flushed = await queuedSink.TryFlushNowAsync();

          var flushedCount = flushed.Key;
          totalFlushed += flushedCount;

          if (flushed.Value.Length > 0)
          {
            AddJson(report, $"Sink_{sinkType}_FailedLogs", new
            {
              Sink = sinkType,
              FlushedCount = flushedCount,
              FailedLogs = flushed.Value.Select(e => new
              {
                e.Time,
                e.LogLevel,
                e.Message,
                e.Exception,
              }).ToArray()
            });
            errors++;
          }
          else
          {
            AddJson(report, $"Sink_{sinkType}_Flushed", new
            {
              Sink = sinkType,
              FlushedCount = 0
            });
          }
        }
      }
      catch (Exception ex)
      {
        errors++;
        AddJson(report, $"Sink_{sinkType}_Error", new
        {
          Sink = sinkType,
          Exception = ex.GetType().FullName,
          ex.Message,
          ex.StackTrace
        }, ReportInformationSeverity.Error);
      }
    }

    // Summary extras
    AddSimple(report, "Sinks_Total", totalSinks.ToString());
    AddSimple(report, "TotalFlushedEntries", totalFlushed.ToString(),
        totalFlushed > 0 ? ReportInformationSeverity.Warning : ReportInformationSeverity.Info);
    AddSimple(report, "Errors", errors.ToString(),
        errors > 0 ? ReportInformationSeverity.Error : ReportInformationSeverity.Info);

    // Final status/message
    if (errors > 0)
    {
      report.Status = HealthCheckStatus.Failed;
      report.Message = "Errors occurred while probing/flush­ing log sinks.";
    }
    else if (totalFlushed > 0)
    {
      report.Status = HealthCheckStatus.OK; // or Failed, if you prefer stricter behavior
      report.Message = "Queued log entries were found and flushed.";
    }
    else
    {
      report.Status = HealthCheckStatus.OK;
      report.Message = "No queued log entries were detected.";
    }

  }

  // ---- helpers ----
  private static void AddSimple(HealthCheckReportEto report, string key, string value,
      ReportInformationSeverity severity = ReportInformationSeverity.Info)
  {
    report.ExtraInformation.Add(new ReportExtraInformationEto
    {
      Key = key,
      Value = value,
      Severity = severity,
      Type = HealthCheckDefaults.ExtraInfoTypes.Simple
    });
  }

  private static void AddJson(HealthCheckReportEto report, string key, object value,
      ReportInformationSeverity severity = ReportInformationSeverity.Info)
  {
    report.ExtraInformation.Add(new ReportExtraInformationEto
    {
      Key = key,
      Value = JsonSerializer.Serialize(value),
      Severity = severity,
      Type = HealthCheckDefaults.ExtraInfoTypes.Json
    });
  }
}
