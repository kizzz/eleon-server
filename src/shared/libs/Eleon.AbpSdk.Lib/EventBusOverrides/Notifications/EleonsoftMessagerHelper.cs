using EleonsoftSdk.modules.Messager.Module.Abstractions;
using EleonsoftSdk.modules.Messager.Module.Email;
using EleonsoftSdk.modules.Messager.Module.Telegram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.Messager.Module.Statics;

public static class EleonsoftMessagerHelper
{
  public static async Task<bool> SendMessageAsync(IServiceProvider serviceProvider, SendMessageRequest request, string messagerName = "email")
  {
    switch (messagerName.ToLower())
    {
      case "telegram":
        return await TelegramHelper.SendAsync(request.GetBotToken(), request.GetChatId(), request.Message);
      case "email":
      case "sms":
        return await EleonsoftNotificationMessager.SendAsync(serviceProvider, messagerName, request.Message, request.GetRecepients());
      default:
        throw new NotSupportedException($"Messager '{messagerName}' is not supported.");
    }
  }
}
