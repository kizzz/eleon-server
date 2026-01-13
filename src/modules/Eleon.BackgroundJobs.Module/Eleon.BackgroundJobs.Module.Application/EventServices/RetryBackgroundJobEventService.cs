using System;
using System.Threading.Tasks;
using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using VPortal.BackgroundJobs.Module.DomainServices;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.Repositories;

namespace BackgroundJobs.Module.EventServices
{
  public class RetryBackgroundJobEventService
    : IDistributedEventHandler<RetryBackgroundJobMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<RetryBackgroundJobEventService> logger;
    private readonly IDistributedEventBus massTransitPublisher;
    private readonly IObjectMapper objectMapper;
    private readonly ICurrentTenant currentTenant;
    private readonly IBackgroundJobDomainService backgroundJobDomainService;
    private readonly IBackgroundJobsRepository repository;
    private readonly IGuidGenerator guidGenerator;
    private readonly IUnitOfWorkManager unitOfWorkManager;

    public RetryBackgroundJobEventService(
      IVportalLogger<RetryBackgroundJobEventService> logger,
      IDistributedEventBus massTransitPublisher,
      IObjectMapper objectMapper,
      ICurrentTenant currentTenant,
      IBackgroundJobDomainService backgroundJobDomainService,
      IBackgroundJobsRepository repository,
      IGuidGenerator guidGenerator,
      IUnitOfWorkManager unitOfWork
    )
    {
      this.logger = logger;
      this.massTransitPublisher = massTransitPublisher;
      this.objectMapper = objectMapper;
      this.currentTenant = currentTenant;
      this.backgroundJobDomainService = backgroundJobDomainService;
      this.repository = repository;
      this.guidGenerator = guidGenerator;
      this.unitOfWorkManager = unitOfWork;
    }

    public async Task HandleEventAsync(RetryBackgroundJobMsg eventData)
    {
      try
      {
        using var unitOfWork = unitOfWorkManager.Begin(true);
        await backgroundJobDomainService.RetryJob(
          eventData.JobId,
          eventData.StartExecutionParams,
          eventData.StartExecutionExtraParams,
          eventData.TimeoutInMinutes,
          eventData.MaxRetryAttempts,
          eventData.RetryInMinutes,
          eventData.OnFailureRecepients
        );
        await unitOfWork.SaveChangesAsync();
        await unitOfWork.CompleteAsync();
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
