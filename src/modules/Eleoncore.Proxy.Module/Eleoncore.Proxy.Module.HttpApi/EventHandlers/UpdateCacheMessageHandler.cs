using Eleoncore.SDK.CoreEvents;
using EleonsoftProxy.Model;
using Microsoft.Extensions.Logging;
using ProxyRouter.Minimal.HttpApi.Services;

namespace ProxyRouter.Minimal.Host.CoreEvents;

public class UpdateCacheMessageHandler : IMessageHandler
{
  private readonly ILogger<UpdateCacheMessageHandler> logger;
  private readonly ILocationProvider locationProvider;

  public UpdateCacheMessageHandler(
      ILogger<UpdateCacheMessageHandler> logger,
      ILocationProvider locationProvider)
  {
    this.logger = logger;
    this.locationProvider = locationProvider;
  }

  public Task HandleAsync(EventManagementModuleFullEventDto message)
  {
    if (message.Name == "ApplicationUpdatedMsg")
    {
      locationProvider.Clear();
    }

    return Task.CompletedTask;
  }
}
