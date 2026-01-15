using EleonsoftModuleCollector.Commons.Module.Messages.Notificator;
using EleonsoftSdk.modules.Helpers.Module;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Messaging.Module.ETO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Json;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Jobs;

public class SocialNotificationJobParams
{
  public string Platform { get; set; }
  public string ChannelId { get; set; }
  public string Message { get; set; }
  public string TemplateName { get; set; }
  public string TemplateType { get; set; }
}

public class SocialNotificationJob : DefaultBackgroundJob, ITransientDependency
{
  private readonly IDistributedEventBus _eventBus;
  private readonly IJsonSerializer _jsonSerializer;

  public SocialNotificationJob(
      ILogger<DefaultBackgroundJob> logger,
      IDistributedEventBus eventBus,
      IJsonSerializer jsonSerializer) : base(logger, eventBus)
  {
    _eventBus = eventBus;
    _jsonSerializer = jsonSerializer;
  }

  protected override string Type => "SendSocialNotification";

  protected override async Task<JobResult> ProcessExecutionAsync(BackgroundJobEto job, BackgroundJobExecutionEto execution)
  {
    var jobParams = _jsonSerializer.Deserialize<SocialNotificationJobParams>(execution.StartExecutionParams);

    var notification = new EleonsoftNotification
    {
      Recipients = new List<RecipientEto>(),
      Message = jobParams.Message,
      TemplateName = jobParams.TemplateName,
      TemplateType = jobParams.TemplateType,
      RunImmidiate = true,
      Type = new SocialNotificationType
      {
        Platform = jobParams.Platform,
        ChannelId = jobParams.ChannelId
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

    var result = new JobResult(true, "Social notifications successfully sent", new List<BackgroundJobTextInfoEto>());

    return result;
  }
}
