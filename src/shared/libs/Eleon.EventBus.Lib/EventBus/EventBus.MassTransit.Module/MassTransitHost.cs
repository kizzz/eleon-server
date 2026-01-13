using Logging.Module;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventBus.MassTransit.Module
{
  public class MassTransitHost : IAsyncDisposable
  {
    private readonly ILogger<MassTransitHost> logger;
    private readonly IHost host;
    private bool disposed = false;

    public MassTransitHost(ILogger<MassTransitHost> logger, IHost host)
    {
      this.logger = logger;
      this.host = host;
    }

    public void Start()
    {
      logger.LogDebug("{0}.{1} started", nameof(MassTransitHost), nameof(Start));
      try
      {
        _ = Run();
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "{0}.{1} errored", nameof(MassTransitHost), nameof(Start));
        throw;
      }
      finally
      {
        logger.LogDebug("{0}.{1} finished", nameof(MassTransitHost), nameof(Start));
      }
    }

    public IServiceScope CreateScope()
    {
      return host.Services
          .GetRequiredService<IServiceScopeFactory>()
          .CreateScope();
    }

    public async ValueTask DisposeAsync()
    {
      if (disposed) return;

      disposed = true;

      await host.StopAsync();
    }

    private async Task Run()
    {
      await host.StartAsync();
    }
  }
}
