using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Notificators.Implementations;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Logging.Module;
using Messaging.Module.ETO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;
using Volo.Abp.SettingManagement;
using VPortal.Notificator.Module.Settings;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.NotificationHandler;
public class SocialNotificationHandler : INotificationHandler<SocialNotificationType>, ITransientDependency
{
  private readonly IVportalLogger<SocialNotificationHandler> _logger;
  private readonly TelegramNotificator _telegramNotificator;
  private readonly ISettingManager _settingManager;
  private readonly IJsonSerializer _jsonSerializer;

  public SocialNotificationHandler(
      IVportalLogger<SocialNotificationHandler> logger,
      TelegramNotificator telegramNotificator,
      ISettingManager settingManager,
      IJsonSerializer jsonSerializer
      )
  {
    _logger = logger;
    _telegramNotificator = telegramNotificator;
    _settingManager = settingManager;
    _jsonSerializer = jsonSerializer;
  }

  public async Task HandleAsync(EleonsoftNotification notification, SocialNotificationType type)
  {

    try
    {
      if (type.Platform.Equals("telegram", StringComparison.CurrentCultureIgnoreCase))
      {
        var tgSettingsString = await _settingManager.GetOrNullForCurrentTenantAsync(NotificatorModuleSettings.TelegramSettings);
        var tgSettings = _jsonSerializer.Deserialize<TelegramOptions>(tgSettingsString ?? "{}") ?? new TelegramOptions();

        string chatId = string.IsNullOrWhiteSpace(type.ChannelId) ? tgSettings.ChatId : type.ChannelId;
        await _telegramNotificator.SendNotificationAsync(notification.Message, chatId, tgSettings.BotToken);
      }
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
    }
    finally
    {
    }
  }
}
