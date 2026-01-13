using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Logging;
using ModuleCollector.JobScheduler.Module.JobScheduler.Module.Domain.Shared.Constants;
using SharedModule.modules.Helpers.Module.EventHandlers;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;
using VPortal.JobScheduler.Module.DomainServices;

namespace VPortal.JobScheduler.Module.EventServices
{
  public class JobCompletedEventService :
      ConcurrencyAwareEventHandler<BackgroundJobCompletedMsg, JobCompletedEventService>
  {
    private readonly TaskExecutionDomainService domainService;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public JobCompletedEventService(
        IVportalLogger<JobCompletedEventService> logger,
        TaskExecutionDomainService domainService,
        IUnitOfWorkManager unitOfWorkManager)
        : base(logger)
    {
      this.domainService = domainService;
      _unitOfWorkManager = unitOfWorkManager;
    }

    protected override async Task HandleEventInternalAsync(BackgroundJobCompletedMsg eventData)
    {
      // TO DO remove Log Information
      Logger.Log.LogInformation($"JobCompletedEventService - HandleEventAsync started for JobId: {eventData.JobId}");

      using var uow = _unitOfWorkManager.Begin(true);
      var actionExecutions = await domainService.GetRunningExecutionByJobIdAsync(eventData.JobId);

      var executionResult = eventData.CompletionStatus switch
      {
        BackgroundJobStatus.Completed => JobSchedulerExecutionResult.Success,
        BackgroundJobStatus.Cancelled => JobSchedulerExecutionResult.Cancelled,
        _ => JobSchedulerExecutionResult.Fail,
      };

      foreach (var exec in actionExecutions)
      {
        await domainService.AcknowledgeActionCompletedAsync(exec.Id, exec.TaskExecutionId, executionResult, eventData.CompletedBy, eventData.IsManually);
      }
      await uow.SaveChangesAsync();
      await uow.CompleteAsync();
    }
  }
}
