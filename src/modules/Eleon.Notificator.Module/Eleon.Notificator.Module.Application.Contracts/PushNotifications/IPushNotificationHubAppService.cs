using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.PushNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Eleon.Notificator.Module.Eleon.Notificator.Module.Application.Contracts.PushNotifications
{
  public interface IPushNotificationHubControllerService : IApplicationService
  {
    Task<PushNotificationDto> PushNotificationDtoEcho(PushNotificationDto input);
  }
}
