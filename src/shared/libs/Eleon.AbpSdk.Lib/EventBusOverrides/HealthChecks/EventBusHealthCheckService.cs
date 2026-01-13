using Common.EventBus.Module;
using Eleon.Logging.Lib.SystemLog.Logger;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using SharedModule.modules.Logging.Module.SystemLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EventBus.Distributed;

namespace EleonsoftSdk.modules.HealthCheck.Module.General;

public class EventBusHealthCheckService : IHealthCheckService
{
  private readonly IDistributedEventBus _eventBus;

  public EventBusHealthCheckService(
      IDistributedEventBus eventBus)
  {
    _eventBus = eventBus;
  }

  public async Task<bool> AddBulkReportsAsync(List<HealthCheckReportEto> reports, CancellationToken cancellationToken = default)
  {
    try
    {
      var response = await _eventBus.RequestAsync<AddHealthCheckReportBulkResponseMsg>(new AddHealthCheckReportBulkMsg
      {
        HealthCheckReports = reports
      });

      return response.IsSuccess;
    }
    catch (Exception ex)
    {
      EleonsoftLog.Error("Failed to add bulk health check reports", ex);
      return false;
    }
  }

  public async Task<bool> AddReportAsync(HealthCheckReportEto report, CancellationToken cancellationToken = default)
  {
    try
    {
      var response = await _eventBus.RequestAsync<AddHealthCheckReportResponseMsg>(new AddHealthCheckReportMsg
      {
        HealthCheckReport = report
      });

      return response.IsSuccess;
    }
    catch (Exception ex)
    {
      EleonsoftLog.Error("Failed to add health check report", ex);
      return false;
    }
  }

  public async Task<HealthCheckResponse> StartHealthCheckAsync(string type, string initiatorName, CancellationToken cancellationToken = default)
  {
    try
    {
      var result = await _eventBus.RequestAsync<StartHealthCheckResponseMsg>(new StartHealthCheckMsg()
      {
        Type = type,
        InitiatorName = initiatorName
      }, 60);

      return new HealthCheckResponse(true, result.HealthCheckId, string.Empty);
    }
    catch (Exception ex)
    {
      return new HealthCheckResponse(false, Guid.Empty, ex.Message);
    }
  }

  public async Task<HealthCheckResponse> SendHealthCheckAsync(HealthCheckEto healthCheck, CancellationToken cancellationToken = default)
  {
    try
    {
      var result = await _eventBus.RequestAsync<CreateHealthCheckResponseMsg>(new CreateHealthCheckRequestMsg()
      {
        HealthCheck = healthCheck,
      }, 60);

      return new HealthCheckResponse(result.Success, result.HealthCheckId, result.Error);
    }
    catch (Exception ex)
    {
      return new HealthCheckResponse(false, Guid.Empty, ex.Message);
    }
  }
}
