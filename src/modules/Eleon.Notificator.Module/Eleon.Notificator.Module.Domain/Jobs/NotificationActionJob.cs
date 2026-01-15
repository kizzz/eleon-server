using Common.Module.Constants;
using Eleon.Logging.Lib.SystemLog.Contracts;
using EleonsoftModuleCollector.Commons.Module.Messages.Notificator;
using EleonsoftAbp.EleonsoftIdentity.Sessions;
using EleonsoftSdk.modules.Helpers.Module;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Logging.Module;
using Messaging.Module.ETO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Json;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Jobs;

public class NotificationActionJobParams
{
  public List<string> Recipients { get; set; } = new List<string>();
  public string RecipientsType { get; set; }
  public string Message { get; set; }
  public string TemplateName { get; set; }
  public string TemplateType { get; set; }
  public NotificationType NotificationType { get; set; } = NotificationType.Message;
  public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
  public bool? RunImmidiate { get; set; } = true;

  // Message / Push
  public string ApplicationName { get; set; } = "Admin";
  public bool IsLocalizedData { get; set; }
  public bool IsRedirectEnabled { get; set; }
  public string RedirectUrl { get; set; }
  public List<string> DataParams { get; set; } = new List<string>();

  // Social
  public string Platform { get; set; }
  public string ChannelId { get; set; }

  // Email
  public bool IsHtml { get; set; }
  public string Subject { get; set; }
  public Dictionary<string, string> Attachments { get; set; } = new Dictionary<string, string>();

  // System
  public bool WriteLog { get; set; } = true;
  public SystemLogLevel LogLevel { get; set; } = SystemLogLevel.Info;
  public Dictionary<string, string> ExtraProperties { get; set; } = new Dictionary<string, string>();

  // Two Factor
  public string UserName { get; set; }
  public FullSessionInformation Session { get; set; }
}

public class NotificationActionJob : DefaultBackgroundJob, ITransientDependency
{
  private readonly IDistributedEventBus _eventBus;
  private readonly IJsonSerializer _jsonSerializer;

  public NotificationActionJob(
      ILogger<DefaultBackgroundJob> logger,
      IDistributedEventBus eventBus,
      IJsonSerializer jsonSerializer) : base(logger, eventBus)
  {
    _eventBus = eventBus;
    _jsonSerializer = jsonSerializer;
  }

  protected override string Type => "SendNotificationAction";

  protected override async Task<JobResult> ProcessExecutionAsync(BackgroundJobEto job, BackgroundJobExecutionEto execution)
  {
    var jobParams = _jsonSerializer.Deserialize<NotificationActionJobParams>(execution.StartExecutionParams);
    var recipients = new List<RecipientEto>();
    if (!string.IsNullOrEmpty(jobParams.RecipientsType))
    {
      switch (jobParams.RecipientsType.ToLower())
      {
        case "role":
          recipients = jobParams.Recipients.Select(r => new RecipientEto
          {
            RefId = r,
            Type = NotificatorRecepientType.Role
          }).ToList();
          break;
        case "user":
          recipients = jobParams.Recipients.Select(r => new RecipientEto
          {
            RefId = r,
            Type = NotificatorRecepientType.User
          }).ToList();
          break;
        case "orgunit":
          recipients = jobParams.Recipients.Select(r => new RecipientEto
          {
            RefId = r,
            Type = NotificatorRecepientType.OrganizationUnit
          }).ToList();
          break;
        case "direct":
          recipients = jobParams.Recipients.Select(r => new RecipientEto
          {
            RecipientAddress = r,
            Type = NotificatorRecepientType.Direct
          }).ToList();
          break;
        default:
          throw new NotSupportedException($"RecipientsType {jobParams.RecipientsType} is not supported");
      }
    }

    var notification = new EleonsoftNotification
    {
      Recipients = recipients,
      Message = jobParams.Message,
      TemplateName = jobParams.TemplateName,
      TemplateType = jobParams.TemplateType,
      Priority = jobParams.Priority,
      RunImmidiate = jobParams.RunImmidiate,
      Type = jobParams.NotificationType switch
      {
        NotificationType.Email => new EmailNotificationType
        {
          IsHtml = jobParams.IsHtml,
          Attachments = jobParams.Attachments,
          Subject = jobParams.Subject
        },
        NotificationType.Message => new MessageNotificationType
        {
          ApplicationName = jobParams.ApplicationName,
          IsLocalizedData = jobParams.IsLocalizedData,
          IsRedirectEnabled = jobParams.IsRedirectEnabled,
          TemplateName = jobParams.TemplateName,
          RedirectUrl = jobParams.RedirectUrl,
          DataParams = jobParams.DataParams
        },
        NotificationType.System => new SystemNotificationType
        {
          LogLevel = jobParams.LogLevel,
          WriteLog = jobParams.WriteLog,
          ExtraProperties = jobParams.ExtraProperties ?? new Dictionary<string, string>()
        },
        NotificationType.Push => new PushNotificationType
        {
          ApplicationName = jobParams.ApplicationName,
          IsLocalizedData = jobParams.IsLocalizedData,
          DataParams = jobParams.DataParams
        },
        NotificationType.TwoFactor => new TwoFactorNotificationType
        {
          UserName = jobParams.UserName ?? recipients?.FirstOrDefault()?.RecipientAddress,
          Session = jobParams.Session
        },
        NotificationType.Sms => new SmsNotificationType(),
        NotificationType.Social => new SocialNotificationType
        {
          Platform = jobParams.Platform,
          ChannelId = jobParams.ChannelId
        },
        _ => throw new ArgumentOutOfRangeException(nameof(jobParams.NotificationType), jobParams.NotificationType, "NotificationType is not supported"),
      }
    };

    notification.ExtraProperties = job.ExtraProperties;

    var notifications = new List<EleonsoftNotification>
        {
            notification
        };

    await _eventBus.PublishAsync(new SendInternalNotificationsMsg
    {
      Notifications = notifications
    });

    var result = new JobResult(true, "Notifications successfully sended", new List<BackgroundJobTextInfoEto>());

    return result;
  }
}
