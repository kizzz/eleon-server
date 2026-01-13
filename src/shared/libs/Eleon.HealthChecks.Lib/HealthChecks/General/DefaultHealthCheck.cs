using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.HealthCheck.Module.Base;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Common.Lib.modules.HealthCheck.Module.General;
public abstract class DefaultHealthCheck : IHealthCheck, IEleonsoftHealthCheck
{
  private readonly ILogger<DefaultHealthCheck> _logger;

  private HealthCheckReportEto _lastReport = null!;

  protected DefaultHealthCheck(IServiceProvider serviceProvider)
  {
    ServiceProvider = serviceProvider;
    _logger = serviceProvider.GetRequiredService<ILogger<DefaultHealthCheck>>();
  }

  public abstract string Name { get; }

  public abstract bool IsPublic { get; }
  public IServiceProvider ServiceProvider { get; }


  public abstract Task ExecuteCheckAsync(HealthCheckReportEto report);

  public async Task<HealthCheckReportEto> CheckAsync(Guid healthCheckId)
  {
    var report = new HealthCheckReportEto
    {
      HealthCheckId = healthCheckId,
      CheckName = this.Name,
      IsPublic = this.IsPublic,
      ExtraInformation = new List<ReportExtraInformationEto>(),
      Message = string.Empty,
      Status = EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants.HealthCheckStatus.InProgress,
    };

    _logger.LogDebug("Starting health check execution for {HealthCheckName}", this.Name);

    try
    {
      await ExecuteCheckAsync(report);

      return report;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while executing health check {HealthCheckName}", this.Name);
      report.Status = HealthCheckStatus.Failed;
      report.Message = $"Health check execution failed: {ex.Message}";
      report.ExtraInformation.AddRange(new ReportExtraInformationEto
      {
        Key = "Exception",
        Value = ex.Message,
        Severity = ReportInformationSeverity.Error,
        Type = "Simple",
      },
      new ReportExtraInformationEto
      {
        Key = "StackTrace",
        Value = ex.StackTrace ?? string.Empty,
        Severity = ReportInformationSeverity.Error,
        Type = "Simple",
      });

      return report;
    }
    finally
    {
      _logger.LogDebug("Completed health check execution for {HealthCheckName} with status {Status}", this.Name, report.Status);
    }
  }

  public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
  {
    if (_lastReport == null)
    {
      _lastReport = await CheckAsync(Guid.Empty);
    }

    var description = $"{this.Name}: {_lastReport.Message}";

    var data = _lastReport.ExtraInformation.ToDictionary(
        info => info.Key,
        info => (object)info.Value);

    var result = _lastReport.Status switch
    {
      HealthCheckStatus.OK => HealthCheckResult.Healthy(description, data),
      HealthCheckStatus.InProgress => HealthCheckResult.Degraded(description, null, data),
      HealthCheckStatus.Failed => HealthCheckResult.Unhealthy(description, null, data),
      _ => HealthCheckResult.Unhealthy("Unknown health check status"),
    };

    return result;
  }
}
