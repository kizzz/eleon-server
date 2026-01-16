using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;
using VPortal.JobScheduler.Module.Entities;

namespace VPortal.JobScheduler.Module.DomainServices
{


  public class TaskExecutionManager : DomainService
  {
    private static bool isRunningDueTasks = false;
    private static object runningDueTasksLocker = new object();

    private readonly IVportalLogger<TaskExecutionManager> logger;
    private readonly TaskDomainService taskService;
    private readonly TaskExecutionDomainService taskExecutionService;
    private readonly ITenantRepository tenantRepository;
    private readonly ICurrentTenant currentTenant;
    private readonly IDistributedEventBus messagePublisher;

    public TaskExecutionManager(
        IVportalLogger<TaskExecutionManager> logger,
        TaskDomainService taskDomainService,
        TaskExecutionDomainService taskExecutionService,
        ITenantRepository tenantRepository,
        ICurrentTenant currentTenant,
        IDistributedEventBus messagePublisher)
    {
      this.logger = logger;
      this.taskService = taskDomainService;
      this.taskExecutionService = taskExecutionService;
      this.tenantRepository = tenantRepository;
      this.currentTenant = currentTenant;
      this.messagePublisher = messagePublisher;
    }

    public async Task<bool> RunTaskManuallyAsync(Guid taskId)
    {
      try
      {
        return await taskExecutionService.RequestTaskExecutionAsync(taskId, manual: true, runnedTrigger: null);
      }
      catch (Exception e)
      {
        logger.Capture(e);
        return false;
      }
      finally
      {
      }
    }

    public async Task<bool> StopTaskAsync(Guid taskId)
    {

      try
      {
        var result = await taskExecutionService.StopTaskExecutionAsync(taskId, true);

        return result;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        return false;
      }
      finally
      {
      }
    }


    #region RunDueTasksAsync

    public async Task<bool> RunDueTasksAsync()
    {
      lock (runningDueTasksLocker)
      {
        if (isRunningDueTasks)
        {
          return false;
        }

        isRunningDueTasks = true;
      }

      try
      {
        var allTenants = await tenantRepository.GetListAsync();

        foreach (var tenant in allTenants.Concat([null]))
        {
          try
          {
            using (currentTenant.Change(tenant?.Id))
            {
              var tenantDueTasks = await taskService.GetDueTasksAsync();
              var successfullyRunned = await RequestTenantDueTasksExecutionAsync(tenantDueTasks);
            }
          }
          catch (Exception e)
          {
            logger.CaptureAndSuppress(new Exception($"Failed to run tenant {tenant?.Id} tasks", e));
          }
        }

        return true;
      }
      catch (Exception e)
      {
        logger.CaptureAndSuppress(e);
        return false;
      }
      finally
      {
        lock (runningDueTasksLocker)
        {
          isRunningDueTasks = false;
        }
      }
    }

    private async Task<int> RequestTenantDueTasksExecutionAsync(List<(TaskEntity Task, TriggerEntity Trigger)> dueTasks)
    {
      int runnedAmmount = 0;
      try
      {
        foreach (var dueTask in dueTasks)
        {
          try
          {
            bool requested = await taskExecutionService.RequestTaskExecutionAsync(dueTask.Task.Id, false, dueTask.Trigger);
            if (requested)
            {
              runnedAmmount++;
            }
            else
            {
              throw new Exception($"Failed to run task {dueTask}");
            }
          }
          catch (Exception ex)
          {
            logger.CaptureAndSuppress(new Exception($"Failed to run task {dueTask}", ex));
            continue;
          }
        }
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }

      finally
      {
      }

      return runnedAmmount;
    }

    #endregion
  }
}
