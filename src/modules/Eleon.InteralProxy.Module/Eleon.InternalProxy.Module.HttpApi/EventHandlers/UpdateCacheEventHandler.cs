using Logging.Module;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Messaging.Module.SystemMessages;
using ProxyRouter.Minimal.HttpApi.Services;

namespace VPortal.ProxyRouter;

public class UpdateCacheEventHandler : IDistributedEventHandler<ApplicationUpdatedMsg>, ITransientDependency
{
  private readonly IVportalLogger<UpdateCacheEventHandler> logger;
  private readonly ILocationProvider locationProvider;

  public UpdateCacheEventHandler(
      IVportalLogger<UpdateCacheEventHandler> logger,
      ILocationProvider locationProvider)
  {
    this.logger = logger;
    this.locationProvider = locationProvider;
  }

  public async Task HandleEventAsync(ApplicationUpdatedMsg eventData)
  {
    try
    {
      locationProvider.Clear();
    }
    catch (Exception ex)
    {
      logger.CaptureAndSuppress(ex);
    }
    finally
    {
    }
  }
}
