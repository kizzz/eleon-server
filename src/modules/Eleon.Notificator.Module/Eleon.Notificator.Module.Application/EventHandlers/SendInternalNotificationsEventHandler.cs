using EleonsoftModuleCollector.Commons.Module.Messages.Notificator;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Managers;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Notificator.Module.DomainServices;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.EventHandlers;
public class SendInternalNotificationsEventHandler : IDistributedEventHandler<SendInternalNotificationsMsg>, ITransientDependency
{
  private readonly NotificationMananger _manager;
  private readonly IVportalLogger<SendInternalNotificationsEventHandler> _logger;

  public SendInternalNotificationsEventHandler(
      NotificationMananger manager,
      IVportalLogger<SendInternalNotificationsEventHandler> logger)
  {
    _manager = manager;
    _logger = logger;
  }
  public async Task HandleEventAsync(SendInternalNotificationsMsg eventData)
  {

    try
    {
      foreach (var message in eventData.Notifications)
      {
        try
        {
          await _manager.SendAsync(message);
        }
        catch (Exception ex)
        {
          _logger.CaptureAndSuppress(ex);
        }
      }
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }
  }
}
