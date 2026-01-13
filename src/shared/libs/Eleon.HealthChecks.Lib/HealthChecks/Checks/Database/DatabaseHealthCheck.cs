using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using HealthCheckModule.Module.Domain.Shared.Constants;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Eleon.Common.Lib.modules.HealthCheck.Module.General;

namespace EleonsoftSdk.modules.HealthCheck.Module.Implementations.CheckDatabase;
public class DatabaseHealthCheck : DefaultHealthCheck
{
  public DatabaseHealthCheck(IConfiguration configuration, IServiceProvider serviceProvider) : base(serviceProvider)
  {
    Configuration = configuration;
  }

  public override string Name => "DatabaseHealth";
  public override bool IsPublic => false;
  public IConfiguration Configuration { get; }

  public override async Task ExecuteCheckAsync(HealthCheckReportEto report)
  {
    var csSection = Configuration.GetSection("ConnectionStrings");
    var pairs = csSection.GetChildren()
        .Select(s => new { Name = s.Key, Value = s.Value })
        .Where(p => !string.IsNullOrWhiteSpace(p.Value))
        .ToList();

    var results = new List<ConnResult>();

    foreach (var p in pairs)
    {
      var success = false;
      string error = null;
      long elapsedMs = 0;

      try
      {
        var sw = Stopwatch.StartNew();
        using var conn = new SqlConnection(p.Value);
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT 1";
        await cmd.ExecuteScalarAsync();
        sw.Stop();

        elapsedMs = sw.ElapsedMilliseconds;
        success = true;
      }
      catch (Exception ex)
      {
        success = false;
        error = $"{ex.GetType().Name}: {ex.Message}";
      }

      results.Add(new ConnResult
      {
        Name = p.Name,
        Success = success,
        Error = error,
        ElapsedMs = elapsedMs
      });
    }

    var total = results.Count;
    var ok = results.Count(r => r.Success);
    var failed = total - ok;

    var status = failed == 0
        ? HealthCheckStatus.OK
        : ok > 0
            ? HealthCheckStatus.Failed
            : HealthCheckStatus.Failed;

    var message = total == 0
        ? "No connection strings found under Configuration:ConnectionStrings."
        : $"Checked {total} connection string(s): {ok} OK, {failed} failed.";

    report.Status = status;
    report.Message = message;

    report.ExtraInformation.AddRange(
        results.Select(x => new ReportExtraInformationEto
        {
          Key = $"Connection_{x.Name}",
          Value = JsonSerializer.Serialize(new { x.Name, Status = x.Success ? "OK" : "FAILED", x.Error, x.ElapsedMs }),
          Type = HealthCheckDefaults.ExtraInfoTypes.Json,
          Severity = x.Success ? ReportInformationSeverity.Info : ReportInformationSeverity.Error,
        }));

    report.ExtraInformation.Add(new ReportExtraInformationEto { Key = "TotalConnections", Value = results.Count.ToString() });
    report.ExtraInformation.Add(new ReportExtraInformationEto { Key = "SuccessfulConnections", Value = results.Count(r => r.Success).ToString() });
    report.ExtraInformation.Add(new ReportExtraInformationEto { Key = "FailedConnections", Value = results.Count(r => !r.Success).ToString() });

  }

  private sealed class ConnResult
  {
    public string Name { get; set; }
    public bool Success { get; set; }
    public string Error { get; set; }
    public long ElapsedMs { get; set; }
  }
}
