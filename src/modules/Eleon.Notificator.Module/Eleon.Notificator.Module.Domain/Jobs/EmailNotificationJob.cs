using Common.Module.Constants;
using EleonsoftModuleCollector.Commons.Module.Messages.Notificator;
using EleonsoftSdk.modules.Helpers.Module;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Messaging.Module.ETO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Json;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Jobs;

public class EmailNotificationJobParams
{
  public List<string> Recipients { get; set; } = new List<string>();
  public string RecipientsType { get; set; }
  public bool IsHtml { get; set; }
  public string Subject { get; set; }
  public string Message { get; set; }
  public string TemplateName { get; set; }
  public string TemplateType { get; set; }
}

public class EmailNotificationJob : DefaultBackgroundJob, ITransientDependency
{
  private readonly IDistributedEventBus _eventBus;
  private readonly IJsonSerializer _jsonSerializer;

  public EmailNotificationJob(
      ILogger<DefaultBackgroundJob> logger,
      IDistributedEventBus eventBus,
      IJsonSerializer jsonSerializer) : base(logger, eventBus)
  {
    _eventBus = eventBus;
    _jsonSerializer = jsonSerializer;
  }

  protected override string Type => "SendEmailNotification";

  protected override async Task<JobResult> ProcessExecutionAsync(BackgroundJobEto job, BackgroundJobExecutionEto execution)
  {
    var jobParams = _jsonSerializer.Deserialize<EmailNotificationJobParams>(execution.StartExecutionParams);
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
      RunImmidiate = true,
      Type = new EmailNotificationType
      {
        IsHtml = jobParams.IsHtml,
        Subject = jobParams.Subject
      }
    };

    notification.ExtraProperties = job.ExtraProperties;
    notification.ExtraProperties["JobExecutionId"] = execution.Id.ToString();

    var notifications = new List<EleonsoftNotification>
        {
            notification
        };

    await _eventBus.PublishAsync(new SendInternalNotificationsMsg
    {
      Notifications = notifications
    });

    var result = new JobResult(true, "Email notifications successfully sent", new List<BackgroundJobTextInfoEto>());

    return result;
  }
}
