using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.ValueObjects;
using System;
using System.Threading.Tasks;

namespace VPortal.Notificator.Module.PushNotifications
{
  public interface IPushNotificationHubContext
  {
    Task PushNotification(List<Guid> targetUsers, PushNotificationValueObject notification);
  }
}
