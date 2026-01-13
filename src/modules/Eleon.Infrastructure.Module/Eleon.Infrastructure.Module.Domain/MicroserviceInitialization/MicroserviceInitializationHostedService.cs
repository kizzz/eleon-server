using Eleon.Logging.Lib.VportalLogging;
using Logging.Module;
using Microsoft.Extensions.Hosting;

namespace Authorization.Module.MicroserviceInitialization
{
  public class MicroserviceInitializationHostedService : IHostedService
  {
    private readonly MicroserviceInitializer initializer;
    private readonly IVportalLogger<MicroserviceInitializationHostedService> logger;
    private readonly IBoundaryLogger boundaryLogger;

    public MicroserviceInitializationHostedService(
        MicroserviceInitializer initializer,
        IVportalLogger<MicroserviceInitializationHostedService> logger,
        IBoundaryLogger boundaryLogger)
    {
      this.initializer = initializer;
      this.logger = logger;
      this.boundaryLogger = boundaryLogger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      using var _ = boundaryLogger.Begin("HostedService MicroserviceInitialization");
      try
      {
        await initializer.InitializeMicroservice(null);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      return Task.CompletedTask;
    }
  }
}
