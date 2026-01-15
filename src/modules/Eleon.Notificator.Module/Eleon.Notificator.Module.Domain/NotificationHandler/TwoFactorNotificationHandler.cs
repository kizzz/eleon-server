using Common.EventBus.Module;
using Commons.Module.Messages.Templating;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Managers;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Notificators;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Logging.Module;
using Messaging.Module.ETO;
using ModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Templating.Module.Messages;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Notificator.Module.Notificators.Implementations;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.NotificationHandler;
public class TwoFactorNotificationHandler : INotificationHandler<TwoFactorNotificationType>, ITransientDependency
{
  private readonly EmailNotificator _emailNotificator;
  private readonly SmsNotificator _smsNotificator;
  private readonly DebugNotificator _debugNotificator;
  private readonly IVportalLogger<TwoFactorNotificationHandler> _logger;
  private readonly NotificatorHelperService _helperService;
  private readonly IDistributedEventBus _eventBus;

  public TwoFactorNotificationHandler(
        EmailNotificator emailNotificator,
        SmsNotificator smsNotificator,
        DebugNotificator debugNotificator,
        IVportalLogger<TwoFactorNotificationHandler> logger,
        NotificatorHelperService notificatorHelperService,
        IDistributedEventBus eventBus)
  {
    _emailNotificator = emailNotificator;
    _smsNotificator = smsNotificator;
    _debugNotificator = debugNotificator;
    _logger = logger;
    _helperService = notificatorHelperService;
    _eventBus = eventBus;
  }
  public async Task HandleAsync(EleonsoftNotification notification, TwoFactorNotificationType type)
  {
    var phonses = new List<string>();
    var emails = new List<string>();

    foreach (var recipient in notification.Recipients)
    {
      if (recipient.Type == Common.Module.Constants.NotificatorRecepientType.Direct)
      {
        if (_helperService.IsEmail(recipient.RecipientAddress))
        {
          emails.Add(recipient.RecipientAddress);
        }
        else if (_helperService.IsPhone(recipient.RecipientAddress))
        {
          phonses.Add(recipient.RecipientAddress);
        }
      }
      else if (recipient.Type == Common.Module.Constants.NotificatorRecepientType.User)
      {
        // we can resolve both email and phone from user
        var email = await _helperService.GetAddressAsync(recipient.Type, recipient.RefId, NotificatorAddressType.Email, recipient.RecipientAddress);
        if (!string.IsNullOrWhiteSpace(email))
        {
          emails.Add(email);
        }
        var phone = await _helperService.GetAddressAsync(recipient.Type, recipient.RefId, NotificatorAddressType.Phone, recipient.RecipientAddress);
        if (!string.IsNullOrWhiteSpace(phone))
        {
          phonses.Add(phone);
        }
      }
    }

    var args = new Dictionary<string, string>()
        {
            { "code", notification.Message }
        };
    try
    {
      await _emailNotificator.SendEmailAsync($"{_helperService.GetTenantName()} Two Factor", notification.Message, emails, true);
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
    }

    try
    {
      var template = "Please use the following One-Time Password (OTP) to proceed: {code}";
      var smsMessage = DynamicTemplateRenderer.RenderTemplate(template, args);
      await _smsNotificator.SendSmsAsync(phonses, smsMessage);
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
}
