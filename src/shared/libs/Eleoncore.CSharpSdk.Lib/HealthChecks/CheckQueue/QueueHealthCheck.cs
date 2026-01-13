using Eleon.Common.Lib.modules.HealthCheck.Module.General;
using Eleoncore.SDK.CoreEvents;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftProxy.Api;
using EleonsoftProxy.Model;
using EleonsoftSdk.modules.HealthCheck.Module.Base;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleoncoreAspNetCoreSdk.HealthChecks.CheckQueue;
public class QueueHealthCheck : DefaultHealthCheck
{
  private readonly IQueueApi _queueApi;
  private readonly IEventApi _eventApi;
  private readonly IServiceProvider _serviceProvider;

  public QueueHealthCheck(IQueueApi queueApi, IEventApi eventApi, IServiceProvider serviceProvider) : base(serviceProvider)
  {
    _queueApi = queueApi;
    _eventApi = eventApi;
    _serviceProvider = serviceProvider;
  }

  public override string Name => "QueueHealthCheck";

  public override bool IsPublic => true;

  public override async Task ExecuteCheckAsync(HealthCheckReportEto report)
  {
    var status = HealthCheckStatus.OK;
    string message = "Queues successfuly checked";
    var extraProperties = new List<ReportExtraInformationEto>();

    try
    {
      var queues = _serviceProvider.GetService<IOptions<ScheduleQueueOptions>>()?.Value?.Queues;

      if (queues == null || queues.Count == 0)
      {
        message = "No queues configured for scheduling.";
        extraProperties.Add(new ReportExtraInformationEto { Key = "configIsNull", Value = (queues == null).ToString(), Severity = ReportInformationSeverity.Warning });
        extraProperties.Add(new ReportExtraInformationEto { Key = "queuesCount", Value = (queues?.Count ?? 0).ToString(), Severity = ReportInformationSeverity.Warning });
      }
      else
      {
        _queueApi.UseApiAuth();
        foreach (var queue in queues)
        {
          var response = await _queueApi.EventManagementModuleQueueEnsureCreatedAsync(
              new EventManagementModuleCreateQueueRequestDto
              {
                Name = queue.QueueName,
                DisplayName = queue.QueueName,
                Forwarding = queue.Forwarding,
                MessagesLimit = queue.MessagesLimit,
              });

          if (response.IsSuccessStatusCode)
          {
            extraProperties.Add(new ReportExtraInformationEto { Key = $"Queue_{queue.QueueName}_Ensured", Value = "true" });
          }
          else
          {
            if (status != HealthCheckStatus.Failed)
            {
              message = $"Failed to ensure queue '{queue.QueueName}'.";
            }
            else
            {
              message += $" Also failed to ensure queue '{queue.QueueName}'.";
            }
            status = HealthCheckStatus.Failed;
            extraProperties.Add(new ReportExtraInformationEto { Key = $"Queue_{queue.QueueName}_Ensured", Value = "false" });
          }
        }
      }
    }
    catch (Exception ex)
    {
      status = HealthCheckStatus.Failed;
      message = $"Queue or Event API is not healthy: {ex.Message}";
    }

    report.Status = status;
    report.Message = string.IsNullOrWhiteSpace(message) ? "Queue and Event APIs are healthy" : message;
    report.ExtraInformation = extraProperties;
  }
}


public static class QueueHealthCheckExtensions
{
  public static IServiceCollection AddQueueHealthCheck(this IServiceCollection services)
  {
    services.AddTransient<IEleonsoftHealthCheck, QueueHealthCheck>();
    return services;
  }
}
