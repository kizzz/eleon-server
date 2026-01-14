using Common.Module.Constants;
using EleonsoftSdk.modules.Helpers.Module;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Microsoft.Extensions.Configuration;

namespace Messaging.Module.ETO;

public class RecipientEto
{
  public string RefId { get; set; } // for User and Role type
  public string RecipientAddress { get; set; } // for Direct type
  public NotificatorRecepientType Type { get; set; }
}

public class EleonsoftNotification
{
  public Guid? Id { get; set; }
  public List<RecipientEto> Recipients { get; set; }

  public string Message { get; set; }

  public AbstractNotificationType Type { get; set; }

  public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

  /// <summary> If false sends message with background job. Recommended true for important messages </summary>
  public bool? RunImmidiate { get; set; }
  public Dictionary<string, string> ExtraProperties { get; set; }
  public string TemplateName { get; set; }
  public string TemplateType { get; set; }


  public EleonsoftNotification()
  {
    Recipients = new List<RecipientEto>();
  }
}
