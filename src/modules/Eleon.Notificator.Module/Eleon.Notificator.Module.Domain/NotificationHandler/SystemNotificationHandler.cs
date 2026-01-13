using Common.EventBus.Module;
using Common.Module.Constants;
using Commons.Module.Messages.Templating;
using Eleon.Logging.Lib.SystemLog.Contracts;
using Eleon.Logging.Lib.SystemLog.Logger;
using EleonsoftAbp.Auth;
using EleonsoftModuleCollector.Commons.Module.Constants;
using EleonsoftModuleCollector.Commons.Module.Messages.SystemLog;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Managers;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Notificators;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Notificators.Implementations;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Settings;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.Options;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Logging.Module;
using Messaging.Module.ETO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Migrations.Module;
using ModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.Constants;
using SharedModule.modules.Logging.Module.SystemLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Templating.Module.Messages;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Json;
using Volo.Abp.SettingManagement;
using Volo.Abp.Users;
using VPortal.Notificator.Module.Notificators.Implementations;
using VPortal.Notificator.Module.Settings;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.NotificationHandler;
public class SystemNotificationHandler : INotificationHandler<SystemNotificationType>, ITransientDependency
{
  private readonly EmailNotificator _emailNotificator;
  private readonly TelegramNotificator _telegramNotificator;
  private readonly PushNotificator _pushNotificator;
  private readonly IVportalLogger<SystemNotificationHandler> _logger;
  private readonly DebugNotificator _debugNotificator;
  private readonly NotificatorHelperService _helperService;
  private readonly ISettingManager _settingManager;
  private readonly IdentityUserManager _userManager;
  private readonly IJsonSerializer _jsonSerializer;
  private readonly IDistributedEventBus _distributedEventBus;
  private readonly ICurrentUser _currentUser;
  private readonly IConfiguration _configuration;

  public SystemNotificationHandler(
      EmailNotificator emailNotificator,
      TelegramNotificator telegramNotificator,
      PushNotificator pushNotificator,
      IVportalLogger<SystemNotificationHandler> logger,
      DebugNotificator debugNotificator,
      NotificatorHelperService notificatorHelperService,
      ISettingManager settingManager,
      IdentityUserManager identityUserManager,
      IJsonSerializer jsonSerializer,
      IDistributedEventBus distributedEventBus,
      ICurrentUser currentUser,
      IConfiguration configuration)
  {
    _emailNotificator = emailNotificator;
    _telegramNotificator = telegramNotificator;
    _pushNotificator = pushNotificator;
    _logger = logger;
    _debugNotificator = debugNotificator;
    _helperService = notificatorHelperService;
    _settingManager = settingManager;
    _userManager = identityUserManager;
    _jsonSerializer = jsonSerializer;
    _distributedEventBus = distributedEventBus;
    _currentUser = currentUser;
    _configuration = configuration;
  }

  public async Task HandleAsync(EleonsoftNotification notification, SystemNotificationType type)
  {
    // to prevent loops of notification error -> log -> notify sink -> notification error
    // set state disable notification
    using (_logger.Log.BeginScope(new Dictionary<string, object> { [EleonsoftLog.DisableNotificationProperty] = true }))
    {
      try
      {
        await WriteSystemLogAsync(notification, type);

        var genealTenantSettings = await _settingManager.GetOrDefaultForCurrentTenantAsync<GeneralNotificatorOptions>(NotificatorModuleSettings.GeneralSettings);

        try
        {
          if (genealTenantSettings.SendErrors)
          {
            var replacements = NotificatorHelperService.GetPlaceholdersReplacements(notification, type);
            var emailMessage = await _distributedEventBus.RequestAsync<RenderNotificationTemplateResponse>(new RenderNotificationTemplateMsg
            {

              Placeholders = replacements,
              TemplateName = "Notification Email"
            });
            await NotifyEmailAsync(notification, type, emailMessage.RenderedTemplate);


            var tgMessage = await _distributedEventBus.RequestAsync<RenderNotificationTemplateResponse>(new RenderNotificationTemplateMsg
            {
              Placeholders = replacements,
              TemplateName = "Notification Telegram"
            });
            await NotifySystemTelegramAsync(notification, type, tgMessage.RenderedTemplate);
          }
        }
        catch (Exception ex)
        {
          _logger.CaptureAndSuppress(ex);
        }

        try
        {
          await _debugNotificator.DebugAsync(notification);
        }
        catch (Exception ex)
        {
          _logger.CaptureAndSuppress(ex);
        }
      }
      catch (Exception ex)
      {
        // all errors must be supressed before exiting this scope to prevent a loop
        _logger.Log.LogError(ex, "Failed to handle system notification");
      }
    }
  }

  private async Task WriteSystemLogAsync(EleonsoftNotification notification, SystemNotificationType type)
  {
    try
    {
      if (!type.WriteLog)
      {
        return;
      }

      type.ExtraProperties.TryGetValue("ApplicationName", out var applicationName);

      await _distributedEventBus.PublishAsync(new AddSystemLogMsg
      {
        Logs = new List<AddSystemLogEto>
                {
                    new AddSystemLogEto
                    {
                        ApplicationName = applicationName ?? "Admin",
                        LogLevel = type.LogLevel,
                        Message = notification.Message,
                        ExtraProperties = new Dictionary<string, object>(type.ExtraProperties.ToDictionary(x => x.Key, x => (object)x.Value))
                        {
                            { "NotificationId", notification.Id?.ToString() ?? "" }
                        }
                    }
                }
      });
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
    }
  }

  private async Task NotifyEmailAsync(EleonsoftNotification notification, SystemNotificationType type, string formattedMessage)
  {
    try
    {
      var general = await _settingManager.GetOrDefaultForCurrentTenantAsync<GeneralNotificatorOptions>(NotificatorModuleSettings.GeneralSettings);

      var emails = new List<string>();
      emails.AddRange(await GetRecipientEmailsAsync(notification, type));
      emails.AddRange(await GetSystemEmailsAsync(notification, type, general));
      emails.AddRange(await GetAdminEmailsAndPushAsync(notification, type, general));
      emails = emails.Distinct().ToList();



      await _emailNotificator.SendEmailAsync($"{_helperService.GetTenantName()} System Notification", formattedMessage, emails, true);
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
    }
  }

  private async Task<List<string>> GetRecipientEmailsAsync(EleonsoftNotification notification, SystemNotificationType type)
  {
    try
    {
      var emails = new List<string>();

      foreach (var recipient in notification.Recipients)
      {
        if (recipient.Type == NotificatorRecepientType.Direct)
        {
          if (_helperService.IsEmail(recipient.RecipientAddress))
          {
            emails.Add(recipient.RecipientAddress);
          }
        }
        else if (recipient.Type == NotificatorRecepientType.User)
        {
          // we can resolve both email and phone from user
          var email = await _helperService.GetAddressAsync(recipient.Type, recipient.RefId, NotificatorAddressType.Email, recipient.RecipientAddress);
          if (!string.IsNullOrWhiteSpace(email))
          {
            emails.Add(email);
          }
        }
      }

      return emails;
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
    }
    return [];
  }

  private async Task<List<string>> GetAdminEmailsAndPushAsync(EleonsoftNotification notification, SystemNotificationType type, GeneralNotificatorOptions general)
  {
    var result = new List<string>();

    try
    {
      if (!ShouldNotify(type.LogLevel, general.MinLogLevel))
      {
        return result;
      }

      var admins = await _userManager.GetUsersInRoleAsync(MigrationConsts.AdminRoleNameDefaultValue);

      result = admins
          .Select(r => r.Email)
          .Where(x => !string.IsNullOrWhiteSpace(x))
          .Distinct()
          .ToList();

      var adminIds = admins.Select(a => a.Id).ToList();
      await _pushNotificator.SendSystemAsync(adminIds, notification.Message, [], false, notification.Priority);
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
    }

    return result;
  }

  private async Task<List<string>> GetSystemEmailsAsync(EleonsoftNotification notification, SystemNotificationType type, GeneralNotificatorOptions general)
  {
    try
    {
      if (!ShouldNotify(type.LogLevel, general.MinLogLevel))
      {
        return [];
      }

      return general.SystemEmails ?? [];
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
    }

    return [];
  }

  private async Task NotifySystemTelegramAsync(EleonsoftNotification notification, SystemNotificationType type, string formattedMessage)
  {
    try
    {
      var tgSettingsString = await _settingManager.GetOrNullForCurrentTenantAsync(NotificatorModuleSettings.TelegramSettings);
      var tgSettings = _jsonSerializer.Deserialize<TelegramOptions>(tgSettingsString ?? "{}") ?? new TelegramOptions();
      if (tgSettings.Enabled && ShouldNotify(type.LogLevel, tgSettings.MinLogLevel))
      {
        await _telegramNotificator.SendNotificationAsync(formattedMessage, tgSettings.SystemChatId, tgSettings.SystemBotToken);
      }
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
    }
  }

  private static bool ShouldNotify(SystemLogLevel notificationLogLevel, SystemLogLevel minLogLevel)
  {
    return (int)notificationLogLevel >= (int)minLogLevel;
  }
}
