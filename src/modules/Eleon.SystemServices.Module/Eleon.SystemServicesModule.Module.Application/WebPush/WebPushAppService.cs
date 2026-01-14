using Eleon.SystemServices.Module.Full.Eleon.SystemServicesModule.Module.Application.Contracts.WebPush;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.EventBus.Distributed;
using VPortal.SystemServicesModule.Module;

namespace Eleon.SystemServices.Module.Full.Eleon.SystemServicesModule.Module.Application.WebPush;

public class WebPushAppService : SystemServicesModuleAppService, IWebPushAppService
{
  private readonly IDistributedEventBus _eventBus;

  public WebPushAppService(IDistributedEventBus eventBus)
  {
    _eventBus = eventBus;
  }
  public async Task<bool> AddWebPushSubscriptionAsync(WebPushSubscriptionDto subscription)
  {
    // todo: add web push subscription
    return true;
  }
}
