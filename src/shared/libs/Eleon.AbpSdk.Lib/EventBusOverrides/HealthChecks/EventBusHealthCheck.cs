using Castle.Core.Configuration;
using Common.EventBus.Module;
using Common.Module.Events;
using Eleon.Common.Lib.modules.HealthCheck.Module.General;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.HealthCheck.Module.Base;
using EleonsoftSdk.modules.HealthCheck.Module.General;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using HealthChecks.UI.Data;
using k8s.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EventBus.Distributed;

namespace EleonsoftSdk.modules.HealthCheck.Module.Implementations.CheckEventBus;


[DistributedEvent]
public class CheckBusMsg
{
  public Guid HealthCheckId { get; set; }
  public DateTime StartTime { get; set; } = DateTime.UtcNow;
}

[DistributedEvent]
public class CheckBusResponseMsg { }


public class EventBusHealthCheck : DefaultHealthCheck
{
  private readonly IDistributedEventBus _eventBus;

  public EventBusHealthCheck(IDistributedEventBus eventBus, IServiceProvider serviceProvider) : base(serviceProvider)
  {
    _eventBus = eventBus;
  }

  public override string Name => "EventBusHealth";

  public override bool IsPublic => true;

  public override async Task ExecuteCheckAsync(HealthCheckReportEto report)
  {
    try
    {
      var stopwatch = System.Diagnostics.Stopwatch.StartNew();
      var result = await _eventBus.RequestAsync<CheckBusResponseMsg>(new CheckBusMsg { HealthCheckId = report.HealthCheckId, StartTime = DateTime.UtcNow });
      stopwatch.Stop();

      report.Status = HealthCheckStatus.OK;
      report.Message = "EventBus is healthy. Response received successfully";
      report.ExtraInformation = new List<ReportExtraInformationEto>
            {
                new ReportExtraInformationEto { Key = "ResponseReceivedTime", Value = DateTime.UtcNow.ToString("o") },
                new ReportExtraInformationEto { Key = "ResponseTimeMs", Value = stopwatch.ElapsedMilliseconds.ToString() }
            };
    }
    catch (Exception ex)
    {
      report.Status = HealthCheckStatus.Failed;
      report.Message = $"EventBus is not healthy: {ex.Message}";
      report.ExtraInformation = new List<ReportExtraInformationEto>
            {
                new ReportExtraInformationEto { Key = "Exception", Value = ex.ToString(), Severity = ReportInformationSeverity.Error },
                new ReportExtraInformationEto { Key = "StackTrace", Value = ex.StackTrace ?? string.Empty, Severity = ReportInformationSeverity.Error },
                new ReportExtraInformationEto { Key = "ErrorTime", Value = DateTime.UtcNow.ToString("o"), Severity = ReportInformationSeverity.Error }
            };
    }
  }
}

public class BusHealthEventHandler : IDistributedEventHandler<CheckBusMsg>
{
  private readonly IResponseContext _responseContext;
  private readonly HealthCheckManager _healthCheckManager;

  public BusHealthEventHandler(IResponseContext responseContext, HealthCheckManager healthCheckManager)
  {
    _responseContext = responseContext;
    _healthCheckManager = healthCheckManager;
  }

  public async Task HandleEventAsync(CheckBusMsg eventData)
  {
    //await _healthCheckManager.AddReportAsync(new HealthCheckReportEto
    //{
    //    HealthCheckId = eventData.HealthCheckId,
    //    CheckName = EventBusHealthCheck.CheckName,
    //    Message = "Message recieved successfully",
    //    ExtraProperties = new Dictionary<string, object>
    //    {
    //        { "EventReceivedTime", DateTime.UtcNow },
    //        { "EventStartTime", eventData.StartTime },
    //        { "LatencyMs", (DateTime.UtcNow - eventData.StartTime).TotalMilliseconds },
    //    },
    //});
    await _responseContext.RespondAsync(new CheckBusResponseMsg());
  }
}
