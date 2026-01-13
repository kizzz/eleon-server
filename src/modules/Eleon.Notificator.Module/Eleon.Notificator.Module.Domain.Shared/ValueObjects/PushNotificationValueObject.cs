using Common.Module.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.ValueObjects;
public class PushNotificationValueObject
{
  public DateTime CreationTime { get; set; }

  public string Content { get; set; }

  public bool IsLocalizedData { get; set; }

  public List<string> DataParams { get; set; }

  public string ApplicationName { get; set; }

  public bool IsRedirectEnabled { get; set; }
  public string RedirectUrl { get; set; }
  public NotificationPriority Priority { get; set; }
  public bool IsNewMessage { get; set; } = true;
}
