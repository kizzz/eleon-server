using EleonsoftSdk.modules.Messager.Module.Abstractions;
using EleonsoftSdk.modules.Messager.Module.Telegram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Volo.Abp.DependencyInjection;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Notificators.Implementations;
public class TelegramNotificator
{
  public async Task<bool> SendNotificationAsync(string message, string chatId, string botToken)
  {
    return await TelegramHelper.SendAsync(botToken, chatId, message);
  }
}
