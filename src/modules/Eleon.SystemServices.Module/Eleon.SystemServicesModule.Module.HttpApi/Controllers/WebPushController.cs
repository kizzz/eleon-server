using Eleon.SystemServices.Module.Full.Eleon.SystemServicesModule.Module.Application.Contracts.SystemLog;
using Eleon.SystemServices.Module.Full.Eleon.SystemServicesModule.Module.Application.Contracts.WebPush;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using VPortal.SystemServicesModule.Module;

namespace Eleon.SystemServices.Module.Full.Eleon.SystemServicesModule.Module.HttpApi.Controllers;

[Area(SystemServicesRemoteServiceConsts.ModuleName)]
[RemoteService(Name = SystemServicesRemoteServiceConsts.RemoteServiceName)]
[Route("api/system-services/web-push")]
public class WebPushController : SystemServicesModuleController, IWebPushAppService
{
  private readonly IWebPushAppService _appService;

  public WebPushController(IWebPushAppService appService)
  {
    _appService = appService;
  }

  [HttpPost("AddWebPushSubscription")]
  public async Task<bool> AddWebPushSubscriptionAsync(WebPushSubscriptionDto subscription)
  {
    return await _appService.AddWebPushSubscriptionAsync(subscription);
  }
}
