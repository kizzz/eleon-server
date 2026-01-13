using Eleon.Logging.Lib.SystemLog.Logger;
using Eleoncore.SDK.CoreEvents;
using EleoncoreAspNetCoreSdk.HealthChecks.CheckQueue;
using EleoncoreAspNetCoreSdk.HealthChecks.SdkCheck;
using EleonsoftProxy.Api;
using EleonsoftProxy.Model;
using EleonsoftSdk.modules.HealthCheck.Module.General;
using EleonsoftSdk.modules.HealthCheck.Module.General.BackgroundExecution;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SharedModule.modules.Logging.Module.SystemLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleoncoreAspNetCoreSdk.HealthChecks.Overrides;

public class SdkHealthCheckService : IHealthCheckService
{
  private readonly IHealthCheckApi _healthCheckApi;

  public SdkHealthCheckService(IHealthCheckApi healthCheckApi)
  {
    _healthCheckApi = healthCheckApi;
  }

  public async Task<bool> AddBulkReportsAsync(List<HealthCheckReportEto> reports, CancellationToken cancellationToken = default)
  {
    try
    {
      _healthCheckApi.UseApiAuth();
      var response = await _healthCheckApi.EleonsoftModuleCollectorHealthCheckAddReportBulkAsync(new EleonsoftModuleCollectorAddHealthCheckReportBulkDto
      {
        Reports = reports.Select(report => new EleonsoftModuleCollectorAddHealthCheckReportDto
        {
          CheckName = report.CheckName,
          ExtraInformation = report.ExtraInformation.Select(x => new EleonsoftModuleCollectorReportExtraInformationDto
          {
            Key = x.Key,
            Value = x.Value,
            Severity = (EleonsoftSdkReportInformationSeverity)x.Severity
          }).ToList(),
          HealthCheckId = report.HealthCheckId,
          Message = report.Message,
          Status = (EleonsoftModuleCollectorHealthCheckStatus)report.Status,
          ServiceName = report.ServiceName,
          IsPublic = report.IsPublic,
        }).ToList()
      }, cancellationToken);

      return response.IsSuccessStatusCode;
    }
    catch (Exception ex)
    {
      EleonsoftLog.Error("SdkHealthCheckService.AddBulkReportsAsync failed", ex);
      return false;
    }
  }

  public async Task<bool> AddReportAsync(HealthCheckReportEto report, CancellationToken cancellationToken = default)
  {
    try
    {
      _healthCheckApi.UseApiAuth();
      var response = await _healthCheckApi.EleonsoftModuleCollectorHealthCheckAddReportAsync(new EleonsoftModuleCollectorAddHealthCheckReportDto
      {
        CheckName = report.CheckName,
        ExtraInformation = report.ExtraInformation.Select(x => new EleonsoftModuleCollectorReportExtraInformationDto
        {
          Key = x.Key,
          Value = x.Value,
          Severity = (EleonsoftSdkReportInformationSeverity)x.Severity
        }).ToList(),
        HealthCheckId = report.HealthCheckId,
        Message = report.Message,
        Status = (EleonsoftModuleCollectorHealthCheckStatus)report.Status,
        ServiceName = report.ServiceName,
        IsPublic = report.IsPublic,
      }, cancellationToken);

      return response.IsSuccessStatusCode;
    }
    catch (Exception ex)
    {
      EleonsoftLog.Error("SdkHealthCheckService.AddReportAsync failed", ex);
      return false;
    }
  }

  public async Task<HealthCheckResponse> SendHealthCheckAsync(HealthCheckEto healthCheck, CancellationToken cancellationToken = default)
  {
    try
    {
      _healthCheckApi.UseApiAuth();
      var response = await _healthCheckApi.EleonsoftModuleCollectorHealthCheckSendAsync(new EleonsoftModuleCollectorSendHealthCheckDto
      {
        Id = healthCheck.Id,
        InitiatorName = healthCheck.InitiatorName,
        Type = healthCheck.Type,
        Reports = healthCheck.Reports.Select(report => new EleonsoftModuleCollectorHealthCheckReportDto
        {
          CheckName = report.CheckName,
          ExtraInformation = report.ExtraInformation.Select(x => new EleonsoftModuleCollectorReportExtraInformationDto
          {
            Key = x.Key,
            Value = x.Value,
            Severity = (EleonsoftSdkReportInformationSeverity)x.Severity
          }).ToList(),
          HealthCheckId = report.HealthCheckId,
          Message = report.Message,
          Status = (EleonsoftModuleCollectorHealthCheckStatus)report.Status,
          ServiceName = report.ServiceName,
          IsPublic = report.IsPublic,
        }).ToList(),
      }, cancellationToken);

      var result = response.Ok();

      return new HealthCheckResponse(result?.Id.HasValue == true, result?.Id ?? Guid.Empty, response?.ReasonPhrase ?? string.Empty);
    }
    catch (Exception ex)
    {
      return new HealthCheckResponse(false, Guid.Empty, ex.Message);
    }
  }

  public async Task<HealthCheckResponse> StartHealthCheckAsync(string type, string initiatorName, CancellationToken cancellationToken = default)
  {
    try
    {
      _healthCheckApi.UseApiAuth();
      var response = await _healthCheckApi.EleonsoftModuleCollectorHealthCheckCreateAsync(new EleonsoftModuleCollectorCreateHealthCheckDto
      {
        InitiatorName = initiatorName,
        Type = type,
      }, cancellationToken);

      var result = response.Ok();

      return new HealthCheckResponse(result?.Id.HasValue == true, result?.Id ?? Guid.Empty, response?.ReasonPhrase ?? string.Empty);
    }
    catch (Exception ex)
    {
      return new HealthCheckResponse(false, Guid.Empty, ex.Message);
    }
  }
}

public class HealthCheckMessageHandler : MessageHandler<HealthCheckStartedMsg>
{
  private readonly ILogger<HealthCheckMessageHandler> _logger;
  private readonly HealthCheckManager _healthCheckQueue;

  public HealthCheckMessageHandler(ILogger<HealthCheckMessageHandler> logger, HealthCheckManager healthCheckQueue) : base()
  {
    _logger = logger;
    _healthCheckQueue = healthCheckQueue;
  }

  public override async Task HandleAsync(HealthCheckStartedMsg message)
  {
    _logger.LogDebug($"{nameof(HealthCheckMessageHandler)}.{nameof(HandleAsync)} started");

    try
    {
      await _healthCheckQueue.ExecuteHealthCheckAsync(message.Type, message.InitiatorName, message.HealthCheckId);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"{nameof(HealthCheckMessageHandler)}.{nameof(HandleAsync)} failed");
      throw;
    }
    finally
    {
      _logger.LogDebug($"{nameof(HealthCheckMessageHandler)}.{nameof(HandleAsync)} finished");

    }
  }
}


public static class SdkHealthCheckExtensions
{
  public static IServiceCollection UseSdkHealthChecks(this IServiceCollection services)
  {
    services.RemoveAll<IHealthCheckService>();
    services.AddTransient<IHealthCheckService, SdkHealthCheckService>();
    services.AddTransient<IMessageHandler, HealthCheckMessageHandler>();
    return services;
  }

  public static IServiceCollection AddEleoncoreHealthChecks(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddSdkHealthCheck();
    services.AddQueueHealthCheck();
    services.UseSdkHealthChecks();
    return services;
  }
}
