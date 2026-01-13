using Common.Module.Constants;
using Eleon.Logging.Lib.SystemLog.Logger;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.EventBus.Distributed;

namespace EleonsoftSdk.modules.Messager.Module.Email;

public static class EleonsoftNotificationMessager
{
  public static async Task<bool> SendAsync(IServiceProvider serviceProvider, string type, string message, List<string> addresses)
  {
    var eventBus = serviceProvider.GetRequiredService<IDistributedEventBus>();

    try
    {
      var id = Guid.NewGuid();

      var msg = new AddNotificationMsg
      {
        Notification = new EleonsoftNotification
        {
          Id = id,
          Message = message,
          Type = ParseType(type),
          RunImmidiate = true,
          Recipients = GetRecepients(addresses),
          Priority = NotificationPriority.Normal,
        }
      };

      await eventBus.PublishAsync(msg);

      return true;
    }
    catch (Exception ex)
    {
      EleonsoftLog.Error("Error sending notification", ex);
      return false;
    }
  }

  private static List<RecipientEto> GetRecepients(List<string> addresses)
  {
    if (addresses == null || addresses.Count == 0)
    {
      throw new InvalidOperationException("Recepients was not provided");
    }

    return addresses.Select(x => new RecipientEto
    {
      Type = NotificatorRecepientType.Direct,
      RecipientAddress = x,
    }).ToList();
  }

  private static AbstractNotificationType ParseType(string type)
  {
    return type.ToLower() switch
    {
      "email" => new EmailNotificationType() { IsHtml = true },
      "sms" => new SmsNotificationType(),
      _ => throw new InvalidOperationException($"Notification type '{type}' is not supported"),
    };
  }
}
