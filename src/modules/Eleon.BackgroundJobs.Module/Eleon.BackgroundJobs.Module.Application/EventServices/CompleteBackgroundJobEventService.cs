using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using VPortal.BackgroundJobs.Module.DomainServices;

namespace VPortal.BackgroundJobs.Module.EventHandlers
{

  public class CompleteBackgroundJobEventService :
      IDistributedEventHandler<BackgroundJobExecutionCompletedMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<CompleteBackgroundJobEventService> logger;
    private readonly IBackgroundJobDomainService backgroundJobDomainService;
    private readonly IDistributedEventBus messagePublisher;
    private readonly IObjectMapper<ModuleDomainModule> objectMapper;
    private readonly ICurrentTenant currentTenant;

    public CompleteBackgroundJobEventService(
        IVportalLogger<CompleteBackgroundJobEventService> logger,
        IBackgroundJobDomainService backgroundJobDomainService,
        IDistributedEventBus messagePublisher,
        IObjectMapper<ModuleDomainModule> objectMapper,
        ICurrentTenant currentTenant)
    {
      this.logger = logger;
      this.backgroundJobDomainService = backgroundJobDomainService;
      this.messagePublisher = messagePublisher;
      this.objectMapper = objectMapper;
      this.currentTenant = currentTenant;
    }

    public async Task HandleEventAsync(BackgroundJobExecutionCompletedMsg eventData)
    {
      logger.Log.LogInformation($"BacgroundJobExecutionCompleted for execution {eventData.ExecutionId}");
      try
      {
        using (currentTenant.Change(eventData.TenantId))
        {
          await backgroundJobDomainService.CompleteExecutionAsync(
              eventData.BackgroundJobId,
              eventData.ExecutionId,
              eventData.Status == Common.Module.Constants.BackgroundJobExecutionStatus.Completed,
              eventData.ParamsForRetryExecution,
              eventData.ExtraParamsForRetryExecution,
              eventData.Messages?.Select(x => new Entities.BackgroundJobMessageEntity { MessageType = x.Type, TextMessage = x.TextMessage, CreationTime = x.CreationTime }).ToList() ?? new List<Entities.BackgroundJobMessageEntity>(),
              eventData.Result,
              eventData.CompletedBy,
              eventData.IsManually);
        }
      }
      catch (AbpDbConcurrencyException ex)
      {
        // Domain service now handles this gracefully, but log for monitoring
        logger.Log.LogWarning(ex,
            "Concurrency exception in event handler for execution {ExecutionId}, but domain service should have handled it.",
            eventData.ExecutionId);
        // Don't rethrow - domain service treats "already completed" as success
      }
      catch (Exception e)
      {
        logger.Capture(e);
        // Re-throw non-concurrency exceptions for retry/dead-letter handling
        throw;
      }
      finally
      {
      }

    }
  }

}
