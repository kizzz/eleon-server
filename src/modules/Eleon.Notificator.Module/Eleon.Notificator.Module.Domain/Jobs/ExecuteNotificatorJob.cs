using Common.Module.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.BackgroundJobs;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using ModuleCollector.Commons.Module.Constants;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Json;
using VPortal.Notificator.Module.DomainServices;

namespace VPortal.Notificator.Module.Jobs
{
  public class ExecuteNotificatorJob
       :
      IDistributedEventHandler<StartingInternalSystemJobExecutionMsg>,
      ITransientDependency
  {
    private readonly NotificatorDomainService notificatorDomainService;
    private readonly IJsonSerializer jsonSerializer;
    private readonly IVportalLogger<ExecuteNotificatorJob> logger;
    private readonly IDistributedEventBus _eventBus;

    public ExecuteNotificatorJob(
        NotificatorDomainService notificatorDomainService,
        IJsonSerializer jsonSerializer,
        IVportalLogger<ExecuteNotificatorJob> logger,
        IDistributedEventBus eventBus)
    {
      this.notificatorDomainService = notificatorDomainService;
      this.jsonSerializer = jsonSerializer;
      this.logger = logger;
      _eventBus = eventBus;
    }

    public async Task HandleEventAsync(StartingInternalSystemJobExecutionMsg eventData)
    {
      if (eventData?.BackgroundJob.Type != NotificatorBackgroundJobTypes.SendBulkNotification)
      {
        return;
      }


      var job = eventData.BackgroundJob;
      var execution = eventData.Execution;

      await _eventBus.PublishAsync(new MarkJobExecutionStartedMsg
      {
        JobId = job.Id,
        ExecutionId = execution.Id,
        TenantId = eventData.TenantId,
        TenantName = eventData.TenantName,
      });

      var status = BackgroundJobExecutionStatus.Completed;
      string userErrorMessage = null;
      var messages = new List<BackgroundJobTextInfoEto>();
      List<EleonsoftNotification> errors = new List<EleonsoftNotification>();

      try
      {
        List<EleonsoftNotification> notifications = jsonSerializer.Deserialize<List<EleonsoftNotification>>(execution.StartExecutionParams);

        foreach (var notification in notifications)
        {
          try
          {
            var success = await notificatorDomainService.NotifyAsync(notification);

            if (!success)
            {
              errors.Add(notification);
              messages.Add(new BackgroundJobTextInfoEto()
              {
                TextMessage = $"Failed to send notification {notification.Id}",
                Type = Common.Module.Constants.BackgroundJobMessageType.Error,
              });
              status = BackgroundJobExecutionStatus.Errored;
            }
          }
          catch (Exception ex)
          {
            messages.Add(new BackgroundJobTextInfoEto()
            {
              TextMessage = ex.Message,
              Type = Common.Module.Constants.BackgroundJobMessageType.Error,
            });
            status = BackgroundJobExecutionStatus.Errored;
            errors.Add(notification);
          }
        }
      }
      catch (Exception ex)
      {
        userErrorMessage = ex.Message;
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
        if (status != BackgroundJobExecutionStatus.Completed)
        {
          messages.Add(new BackgroundJobTextInfoEto()
          {
            Type = BackgroundJobMessageType.Error,
            TextMessage = userErrorMessage,
          });
        }

        var message = new BackgroundJobExecutionCompletedMsg
        {
          Type = job.Type,
          BackgroundJobId = job.Id,
          ExecutionId = execution.Id,
          ParamsForRetryExecution = jsonSerializer.Serialize(execution.StartExecutionParams),
          ExtraParamsForRetryExecution = execution.StartExecutionExtraParams,
          Status = status,
          Messages = messages,
          TenantId = eventData.TenantId,
          TenantName = eventData.TenantName,
        };

        await _eventBus.PublishAsync(message);

      }
    }
  }
}
