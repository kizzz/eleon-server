using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Notificators.Implementations;
using EleonsoftSdk.modules.Azure;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Logging.Module;
using Messaging.Module.ETO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.Notificator.Module.Emailing;
using VPortal.Notificator.Module.Notificators.Implementations;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Notificators;


public class DebugNotificator
{
  private readonly IConfiguration _configuration;
  private readonly IVportalLogger<DebugNotificator> _logger;
  private readonly SmtpNotificator _smtpNotificator;
  private readonly TelegramNotificator _telegramNotificator;
  private readonly AzureNotificator _azureNotificator;
  private readonly NotificatorBaseHelperService _helperService;


  public DebugNotificator(
      IConfiguration configuration,
      IVportalLogger<DebugNotificator> logger,
      SmtpNotificator smtpNotificator,
      TelegramNotificator telegramNotificator,
      AzureNotificator azureNotificator,
      NotificatorBaseHelperService helperService)
  {
    _configuration = configuration;
    _logger = logger;
    _smtpNotificator = smtpNotificator;
    _telegramNotificator = telegramNotificator;
    _azureNotificator = azureNotificator;
    _helperService = helperService;
  }

  public async Task DebugAsync(EleonsoftNotification notification)
  {
    var cfg = _configuration.GetSection("DebugSettings");

    var isEnabled = cfg.GetValue("Enabled", false);

    if (!isEnabled)
      return;

    var notifyErrors = cfg.GetValue("NotifyErrors", true);

    if (!notifyErrors && notification.Type is SystemNotificationType)
    {
      return;
    }

    var filters = cfg.GetSection("Filters").Get<string[]>();

    if (filters != null && !filters.Contains("*", StringComparer.OrdinalIgnoreCase) && !filters.Contains(notification.Type.Type, StringComparer.OrdinalIgnoreCase))
    {
      return;
    }

    await DebugEmailAsync(cfg, notification);
    await DebugTelegramAsync(cfg, notification);
  }

  private async Task DebugEmailAsync(IConfiguration cfg, EleonsoftNotification notification)
  {
    try
    {
      var isEmailEnabled = cfg.GetValue("EmailTelemetry:Enabled", false);

      if (!isEmailEnabled)
        return;

      var devEmails = cfg.GetSection("EmailTelemetry:DevEmails").Get<List<string>>();

      if (devEmails == null || !devEmails.Any())
        return;

      string message = notification.Message;
      if (notification.Type is SystemNotificationType sysType)
      {
        message = _helperService.RenderSystemMessage(notification, sysType, cfg.GetValue<string>("EmailTelemetry:Template"), cfg.GetValue<string>("EmailTelemetry:TemplateType"));
      }
      else if (notification.Type is TwoFactorNotificationType twoFaType)
      {
        message = $"""
                    <div style="font-family: Arial, sans-serif; color: #333;">
                        <p>Hi,</p>

                        <p>Please use the following One-Time Password (OTP) to proceed:</p>
                        <p>User {twoFaType.UserName}, code {notification.Message}</p>
                        <p>Session information</p>
                        <pre>{twoFaType.Session?.ToString()}</pre>
                    </div>
                    """;
      }

      var smtpOptions = cfg.GetSection("DevSmtp").Get<SmtpEmailSettings>();
      if (smtpOptions != null)
        await _smtpNotificator.SendEmailAsync(smtpOptions, "Development Notification", message, devEmails, smtpOptions.Sender.Address, true, null);

      var azureOptions = cfg.GetSection("DevAzureEws").Get<AzureEwsOptions>();
      if (azureOptions != null)
        await _azureNotificator.SendEmailAsync(azureOptions, "Development Notification", message, devEmails, true, null);
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
    }
  }

  private async Task DebugTelegramAsync(IConfiguration cfg, EleonsoftNotification notification)
  {
    try
    {
      var isTelegramEnabled = cfg.GetValue("TelegramTelemetry:Enabled", false);

      if (!isTelegramEnabled)
        return;

      var chatId = cfg.GetValue<string>("TelegramTelemetry:ChatId");
      var botToken = cfg.GetValue<string>("TelegramTelemetry:BotToken");

      string message = notification.Message;
      if (notification.Type is SystemNotificationType sysType)
      {
        message = _helperService.RenderSystemMessage(notification, sysType, cfg.GetValue<string>("TelegramTelemetry:Template"), cfg.GetValue<string>("TelegramTelemetry:TemplateType"));
      }
      else if (notification.Type is TwoFactorNotificationType twoFaType)
      {
        chatId = cfg.GetValue<string>("TelegramTelemetry:TwoFactorChatId");
        message = $"{_helperService.GetTenantName()}\nUser: {twoFaType.UserName}\n2FA: <code>{notification.Message}</code>\n[{twoFaType.Session?.Request.Host}\n{twoFaType.Session?.Request.ParsedDevice}\n{twoFaType.Session?.Request.XForwardedFor}]";
      }

      if (string.IsNullOrEmpty(chatId) || string.IsNullOrEmpty(botToken) || string.IsNullOrEmpty(message))
        return;

      await _telegramNotificator.SendNotificationAsync(message, chatId, botToken);
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
    }
  }
}
