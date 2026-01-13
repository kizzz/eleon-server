using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EventBus.Distributed;

namespace EleonsoftSdk.modules.HealthCheck.Module.General;
public class HealthCheckEventHandler : IDistributedEventHandler<HealthCheckStartedMsg>
{
  private readonly ILogger<HealthCheckEventHandler> _logger;
  private readonly HealthCheckManager _healthCheckQueue;

  public HealthCheckEventHandler(
      ILogger<HealthCheckEventHandler> logger,
      HealthCheckManager healthCheckQueue)
  {
    _logger = logger;
    _healthCheckQueue = healthCheckQueue;
  }

  public async Task HandleEventAsync(HealthCheckStartedMsg eventData)
  {
    _logger.LogDebug($"{nameof(HealthCheckEventHandler)}.{nameof(HandleEventAsync)} started");

    try
    {
      await _healthCheckQueue.ExecuteHealthCheckAsync(eventData.Type, eventData.InitiatorName, eventData.HealthCheckId);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while handling HealthCheckStartedMsg.");
    }
    finally
    {
      _logger.LogDebug($"{nameof(HealthCheckEventHandler)}.{nameof(HandleEventAsync)} finished");
    }
  }
}
