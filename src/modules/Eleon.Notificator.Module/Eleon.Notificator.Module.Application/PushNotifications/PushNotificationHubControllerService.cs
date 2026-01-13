using Eleon.Notificator.Module.Eleon.Notificator.Module.Application.Contracts.PushNotifications;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.PushNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Eleon.Notificator.Module.Eleon.Notificator.Module.Application.PushNotifications
{
  public class PushNotificationHubControllerService : IPushNotificationHubControllerService, ITransientDependency
  {
    public async Task<PushNotificationDto> PushNotificationDtoEcho(PushNotificationDto input)
    {
      return input;
    }
  }
}
