using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Messaging.Module.ETO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.NotificationHandler;


public interface INotificationHandler<in TType> where TType : AbstractNotificationType
{
  Task HandleAsync(EleonsoftNotification notification, TType type);
}

public interface INotificationHandler
{
  Task HandleAsync(EleonsoftNotification notification);
}

public class NotificationHandler : INotificationHandler, ITransientDependency
{
  private readonly IServiceProvider _sp;

  public NotificationHandler(IServiceProvider sp) => _sp = sp;

  public async Task HandleAsync(EleonsoftNotification notification)
  {
    ArgumentNullException.ThrowIfNull(notification, nameof(notification));
    ArgumentNullException.ThrowIfNull(notification.Type, nameof(notification.Type));

    // Resolve the closed generic handler at runtime:
    var type = notification.Type.GetType();
    var handlerType = typeof(INotificationHandler<>).MakeGenericType(type);

    var handler = _sp.GetService(handlerType)
        ?? throw new InvalidOperationException($"No handler registered for notification type '{type}' ({type.Name}).");

    // Invoke handler
    var handleMethod = handlerType.GetMethod(nameof(INotificationHandler<AbstractNotificationType>.HandleAsync))!;
    await (Task)handleMethod.Invoke(handler, new object[] { notification, notification.Type })!;
  }
}
