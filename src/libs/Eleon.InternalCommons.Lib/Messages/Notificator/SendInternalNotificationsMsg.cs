using Messaging.Module.ETO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.Commons.Module.Messages.Notificator;

public class SendInternalNotificationsMsg
{
  public List<EleonsoftNotification> Notifications { get; set; } = new List<EleonsoftNotification>();

  public SendInternalNotificationsMsg()
  {
    Notifications = new List<EleonsoftNotification>();
  }

  public SendInternalNotificationsMsg(List<EleonsoftNotification> notifications)
  {
    Notifications = notifications ?? new List<EleonsoftNotification>();
  }

  public SendInternalNotificationsMsg(EleonsoftNotification notification)
  {
    ArgumentNullException.ThrowIfNull(notification);
    Notifications = new List<EleonsoftNotification> { notification };
  }
}
