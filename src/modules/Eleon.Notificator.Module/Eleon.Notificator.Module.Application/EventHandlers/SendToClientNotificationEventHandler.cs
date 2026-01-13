using EleonsoftModuleCollector.Commons.Module.Messages;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.EventHandlers;
public class SendToClientNotificationEventHandler : IDistributedEventHandler<SendToClientMsg>, ITransientDependency
{
  private readonly ISystemAppHubContext _hub;

  public SendToClientNotificationEventHandler(ISystemAppHubContext hub)
  {
    _hub = hub;
  }

  public Task HandleEventAsync(SendToClientMsg eventData)
  {
    if (eventData.IsToAll)
    {
      return _hub.SendToAllAsync(eventData.Method, eventData.Data);
    }
    else
    {
      return _hub.SendToAsync(eventData.Method, eventData.Data, eventData.UserIds);
    }
  }
}
