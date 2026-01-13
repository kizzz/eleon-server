using Messaging.Module.Messages;
using Eleon.Logging.Lib.VportalLogging;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.EventBus.Distributed;

namespace VPortal.SitesManagement.Module.Microservices
{
  public class MicroserviceMonitoringService : IHostedService
  {
    private readonly IDistributedEventBus massTransitPublisher;
    private readonly IBoundaryLogger boundaryLogger;

    public MicroserviceMonitoringService(
        IDistributedEventBus massTransitPublisher,
        IBoundaryLogger boundaryLogger)
    {
      this.massTransitPublisher = massTransitPublisher;
      this.boundaryLogger = boundaryLogger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      using var _ = boundaryLogger.Begin("HostedService MicroserviceMonitoringService");
      //var request = new TriggerMicroserviceInitializationMsg();
      //await massTransitPublisher.PublishAsync(request);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      return Task.CompletedTask;
    }
  }
}

