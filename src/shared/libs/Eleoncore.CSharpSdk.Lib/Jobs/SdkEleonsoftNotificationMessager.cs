using Eleon.Logging.Lib.SystemLog.Logger;
using EleonsoftProxy.Api;
using EleonsoftProxy.Model;
using EleonsoftSdk.modules.Helpers.Module;
using EleonsoftSdk.modules.Messager.Module.Abstractions;
using Logging.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedModule.modules.Logging.Module.SystemLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EleoncoreAspNetCoreSdk.Jobs;

public static class SdkEleonsoftNotificationMessager
{
  public static async Task<bool> SendAsync(string type, string message, List<string> addresses)
  {
    try
    {
      var id = Guid.NewGuid();

      var msg = new NotificatorNotificationDto
      {
        Id = id,
        Message = message,
        Type = Enum.Parse<EleoncoreNotificationType>(type),
        RunImmidiate = true,
        Recipients = GetRecepients(addresses),
        ApplicationName = StaticServicesAccessor.GetConfiguration()?.GetValue<string>("NotificationApplication"),
      };

      var _notificationApi = new NotificationsApi();
      _notificationApi.UseApiAuth();

      await _notificationApi.NotificatorNotificationsSendAsync(msg);

      return true;
    }
    catch (Exception ex)
    {
      EleonsoftLog.Error("Error sending notification message", ex);
      throw;
    }
  }

  private static List<NotificatorNotificatorRecepientDto> GetRecepients(List<string> addresses)
  {
    if (addresses == null || addresses.Count == 0)
    {
      throw new InvalidOperationException("Recepients was not provided");
    }

    return addresses.Select(x => new NotificatorNotificatorRecepientDto
    {
      Type = EleoncoreNotificatorRecepientType.Direct,
      RecipientAddress = x,
    }).ToList();
  }
}
