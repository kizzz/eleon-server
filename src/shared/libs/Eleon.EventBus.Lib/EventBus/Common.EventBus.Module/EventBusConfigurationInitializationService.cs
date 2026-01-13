using Eleon.Logging.Lib.VportalLogging;
using Microsoft.Extensions.Hosting;

namespace Common.EventBus.Module
{
  public class EventBusConfigurationInitializationService : IHostedService
  {
    private readonly DistributedBusResolver resolver;
    private readonly IBoundaryLogger boundaryLogger;

    public EventBusConfigurationInitializationService(
        DistributedBusResolver resolver,
        IBoundaryLogger boundaryLogger)
    {
      this.resolver = resolver;
      this.boundaryLogger = boundaryLogger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      using var _ = boundaryLogger.Begin("HostedService EventBusConfigurationInitializationService");
      await resolver.EnsureDefaultBusConnected();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      return Task.CompletedTask;
    }
  }
}
