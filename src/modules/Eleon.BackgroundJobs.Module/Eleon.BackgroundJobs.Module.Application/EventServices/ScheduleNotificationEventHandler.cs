using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using ModuleCollector.Commons.Module.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TenantSettings.Module.Helpers;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using VPortal.BackgroundJobs.Module.DomainServices;
using VPortal.BackgroundJobs.Module.Messages;

namespace VPortal.BackgroundJobs.Module.EventHandlers
{
  // TODO: Remove
  public class ScheduleNotificationEventHandler :
      IDistributedEventHandler<ScheduleNotificationEvent>,
      ITransientDependency
  {
    private readonly IVportalLogger<ScheduleNotificationEventHandler> logger;
    private readonly IBackgroundJobDomainService backgroundJobDomainService;
    private readonly IDistributedEventBus messagePublisher;
    private readonly MultiTenancyDomainService multiTenancyDomainService;
    private readonly ICurrentTenant currentTenant;

    public ScheduleNotificationEventHandler(
        IVportalLogger<ScheduleNotificationEventHandler> logger,
        IBackgroundJobDomainService backgroundJobDomainService,
        IDistributedEventBus messagePublisher,
        MultiTenancyDomainService multiTenancyDomainService,
        ICurrentTenant currentTenant)
    {
      this.logger = logger;
      this.backgroundJobDomainService = backgroundJobDomainService;
      this.messagePublisher = messagePublisher;
      this.multiTenancyDomainService = multiTenancyDomainService;
      this.currentTenant = currentTenant;
    }

    public async Task HandleEventAsync(ScheduleNotificationEvent eventData)
    {
      try
      {
        var jobs = new List<BackgroundJobEto>();
        await multiTenancyDomainService.ForEachTenant(async _ =>
        {
          var tenantJobs = await backgroundJobDomainService.GetByType(NotificatorBackgroundJobTypes.SendBulkNotification);
          var tenantJobsEto = new List<BackgroundJobEto>();
          foreach (var job in tenantJobs)
          {
            job.Status = BackgroundJobStatus.Executing;
            job.LastExecutionDateUtc = DateTime.UtcNow;
            BackgroundJobExecutedMsg executingMessage = new BackgroundJobExecutedMsg
            {
              BackgroundJob = new BackgroundJobEto()
              {
                Id = job.Id,
                Status = job.Status,
                Type = job.Type,
                ScheduleExecutionDateUtc = job.ScheduleExecutionDateUtc,
                IsRetryAllowed = job.IsRetryAllowed,
                Description = job.Description,
                StartExecutionParams = job.StartExecutionParams,
              }
            };
            tenantJobsEto.Add(executingMessage.BackgroundJob);
            await messagePublisher.PublishAsync(executingMessage);
          }

          jobs.AddRange(tenantJobsEto);

          var message = new SendBulkNotificationMsg
          {
            Jobs = jobs
          };
          await messagePublisher.PublishAsync(message);
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
