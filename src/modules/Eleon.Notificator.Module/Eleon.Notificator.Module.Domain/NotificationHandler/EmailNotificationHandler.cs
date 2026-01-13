using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Managers;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Messaging.Module.ETO;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.Notificator.Module.Notificators.Implementations;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.NotificationHandler;
public class EmailNotificationHandler : INotificationHandler<EmailNotificationType>, ITransientDependency
{
  private readonly EmailNotificator _emailNotificator;
  private readonly NotificatorHelperService _helperService;

  public EmailNotificationHandler(EmailNotificator emailNotificator, NotificatorHelperService helperService)
  {
    _emailNotificator = emailNotificator;
    _helperService = helperService;
  }

  public async Task HandleAsync(EleonsoftNotification notification, EmailNotificationType type)
  {
    var emails = new List<string>();

    foreach (var recipient in notification.Recipients)
    {
      var email = await _helperService.GetAddressAsync(recipient.Type, recipient.RefId, NotificatorAddressType.Email, recipient.RecipientAddress);
      if (!string.IsNullOrWhiteSpace(email) && _helperService.IsEmail(email) && !emails.Contains(email))
      {
        emails.Add(email);
      }
    }

    await _emailNotificator.SendEmailAsync(type.Subject, notification.Message, emails, true, type.Attachments);
  }
}
