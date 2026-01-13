using BackgroundJobs.Module.BackgroundJobs;
using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using VPortal.BackgroundJobs.Module.DomainServices;
using VPortal.BackgroundJobs.Module.Entities;

namespace VPortal.BackgroundJobs.Module.EventHandlers
{

  public class CreateBackgroundJobEventService :
      IDistributedEventHandler<CreateBackgroundJobMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<CreateBackgroundJobEventService> logger;
    private readonly IBackgroundJobDomainService backgroundJobDomainService;
    private readonly IDistributedEventBus messagePublisher;
    private readonly IObjectMapper<ModuleDomainModule> objectMapper;
    private readonly ICurrentTenant currentTenant;

    public CreateBackgroundJobEventService(
        IVportalLogger<CreateBackgroundJobEventService> logger,
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

    public async Task HandleEventAsync(CreateBackgroundJobMsg eventData)
    {

      try
      {
        var backgroundJobEntity = await backgroundJobDomainService.CreateAsync(
                eventData.TenantId,
                eventData.Id,
                eventData.ParentJobsIds?.FirstOrDefault(),
                eventData.Type,
                eventData.Initiator,
                eventData.IsRetryAllowed,
                eventData.Description,
                eventData.StartExecutionParams,
                eventData.ScheduleExecutionDateUtc,
                eventData.IsSystemInternal,
                startExecutionExtraParams: eventData.StartExecutionExtraParams,
                sourceId: eventData.SourceId,
                sourceType: eventData.SourceType,
                timeoutInMinutes: eventData.TimeoutInMinutes,
                maxRetryAttempts: eventData.MaxRetryAttempts,
                retryInMinutes: eventData.RetryInMinutes,
                onFailureRecepients: eventData.OnFailureRecepients,
                extraProperties: eventData.ExtraProperties);

        var backgroundJobEto = objectMapper.Map<BackgroundJobEntity, BackgroundJobEto>(backgroundJobEntity);
        await messagePublisher.PublishAsync(new BackgroundJobAddedMsg
        {
          BackgroundJob = backgroundJobEto
        });
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }
  }
}
