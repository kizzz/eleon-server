using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Managers;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Messaging.Module.ETO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.Notificator.Module.Notificators.Implementations;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.NotificationHandler;
public class SmsNotificationHandler : INotificationHandler<SmsNotificationType>, ITransientDependency
{
  private readonly SmsNotificator _smsNotificator;
  private readonly NotificatorHelperService _helperService;

  public SmsNotificationHandler(SmsNotificator smsNotificator, NotificatorHelperService helperService)
  {
    _smsNotificator = smsNotificator;
    _helperService = helperService;
  }

  public async Task HandleAsync(EleonsoftNotification notification, SmsNotificationType type)
  {
    var phones = new List<string>();

    foreach (var recipient in notification.Recipients)
    {
      var phone = await _helperService.GetAddressAsync(recipient.Type, recipient.RefId, NotificatorAddressType.Phone, recipient.RecipientAddress);
      if (!string.IsNullOrWhiteSpace(phone) && _helperService.IsPhone(phone) && !phones.Contains(phone))
      {
        phones.Add(phone);
      }
    }

    await _smsNotificator.SendSmsAsync(phones, notification.Message);
  }
}
