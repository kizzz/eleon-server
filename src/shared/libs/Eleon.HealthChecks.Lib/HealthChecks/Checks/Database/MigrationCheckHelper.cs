using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.HealthCheck.Module.General;
using EleonsoftSdk.modules.Helpers.Module;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.Database;
public static class MigrationCheckHelper
{
  public class MigrationResult
  {
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public Exception? Exception { get; set; }
  }

  public static async Task ExecuteMigrationCheckAsync(Func<MigrationResult, Task> action)
  {
    var migrationResult = true;
    var migrationMessage = "Migration completed successfully";
    var exception = string.Empty;

    try
    {
      var result = new MigrationResult
      {
        Success = migrationResult,
        Message = migrationMessage,
        Exception = null
      };
      await action(result);
      migrationResult = result.Success;
      migrationMessage = result.Message;
      if (result.Exception != null)
      {
        exception = result.Exception.ToString();
      }
    }
    catch (Exception ex)
    {
      migrationResult = false;
      migrationMessage = $"Migration failed: {ex.Message}";
      exception = ex.ToString();
    }
    finally
    {
      var report = new HealthCheckReportEto
      {
        Status = migrationResult ? HealthCheckStatus.OK : HealthCheckStatus.Failed,
        Message = migrationMessage,
        CheckName = "DatabaseMigration",
        ExtraInformation = new List<ReportExtraInformationEto>
                {
                    new ReportExtraInformationEto
                    {
                        Key = "Timestamp",
                        Value = DateTime.UtcNow.ToString("o"),
                        Severity = ReportInformationSeverity.Info
                    }
                }
      };

      if (!string.IsNullOrEmpty(exception))
      {
        report.ExtraInformation.Add(new ReportExtraInformationEto
        {
          Key = "Exception",
          Value = exception,
          Severity = ReportInformationSeverity.Error
        });
      }

      try
      {
        var healthCheckManager = StaticServicesAccessor.GetService<HealthCheckManager>();
        healthCheckManager?.AddGlobalReport(report);
      }
      catch
      {
        // ignore
      }
    }
  }
}
