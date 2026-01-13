using Eleon.Notificator.Module.Eleon.Notificator.Module.Application.Contracts.PushNotifications;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.PushNotifications;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using VPortal.Notificator.Module;
using VPortal.Notificator.Module.Notifications;

namespace Eleon.Notificator.Module.Eleon.Notificator.Module.HttpApi.Controllers
{
  [Area(ModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Notificator/PushNotificationHub")]
  public class PushNotificationHubControllerService : NotificatorModuleController, IPushNotificationHubControllerService
  {
    [HttpGet]
    public async Task<PushNotificationDto> PushNotificationDtoEcho(PushNotificationDto input)
    {
      return input;
    }
  }
}
