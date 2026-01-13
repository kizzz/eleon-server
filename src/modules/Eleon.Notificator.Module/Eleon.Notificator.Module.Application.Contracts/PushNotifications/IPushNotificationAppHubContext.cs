using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.PushNotifications;
using System;
using System.Threading.Tasks;
using VPortal.Notificator.Module.NotificationLogs;

namespace VPortal.Notificator.Module.PushNotifications
{
  public interface IPushNotificationAppHubContext
  {
    public Task NotifyUser(List<Guid> userIds, PushNotificationDto notification);
  }
}
