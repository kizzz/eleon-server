using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.Notificator.Module.Entities;

namespace Eleon.Notificator.Module.Eleon.Notificator.Module.Domain.Shared.Options
{
  public class PushNotificationUserStateOptions
  {
    public Guid? LastAckNotificationLog { get; set; }

    public DateTime? LastAckDate { get; set; }
    public void Acknowledge(NotificationLogEntity notification)
    {
      LastAckNotificationLog = notification.Id;
      LastAckDate = notification.CreationTime;
    }
  }
}
