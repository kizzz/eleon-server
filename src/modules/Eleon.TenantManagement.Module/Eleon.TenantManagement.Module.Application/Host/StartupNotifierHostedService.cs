using Eleon.Logging.Lib.VportalLogging;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.EventBus.Distributed;

namespace VPortal.InitilizationServices;

public class StartupNotifierHostedService : IHostedService
{
  private readonly IVportalLogger<StartupNotifierHostedService> _logger;
  private readonly IDistributedEventBus _eventBus;
  private readonly IBoundaryLogger _boundaryLogger;

  public StartupNotifierHostedService(
      IVportalLogger<StartupNotifierHostedService> logger,
      IDistributedEventBus eventBus,
      IBoundaryLogger boundaryLogger)
  {
    _logger = logger;
    _eventBus = eventBus;
    _boundaryLogger = boundaryLogger;
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    using var _ = _boundaryLogger.Begin("HostedService StartupNotifier");
    try
    {
      await _eventBus.PublishAsync(new UpdateFeaturesPermissionsRequestMsg());
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
    }
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}
