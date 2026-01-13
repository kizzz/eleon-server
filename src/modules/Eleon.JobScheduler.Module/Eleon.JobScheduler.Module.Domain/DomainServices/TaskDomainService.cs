using Common.Module.Constants;
using Logging.Module;
using Microsoft.Extensions.Logging;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using Volo.Abp.Validation;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.Repositories;

namespace VPortal.JobScheduler.Module.DomainServices
{

  public class TaskDomainService : DomainService
  {
    private readonly IVportalLogger<TaskDomainService> logger;
    private readonly TriggerDomainService triggerDomainService;
    private readonly ITaskRepository taskRepository;

    public TaskDomainService(
        IVportalLogger<TaskDomainService> logger,
        ITaskRepository taskRepository,
        TriggerDomainService triggerDomainService)
    {
      this.logger = logger;
      this.taskRepository = taskRepository;
      this.triggerDomainService = triggerDomainService;
    }

    public async Task<TaskEntity> GetByIdAsync(Guid id)
    {
      TaskEntity result = null;
      try
      {
        var task = await taskRepository.GetAsync(id, true);

        result = task;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<KeyValuePair<long, List<TaskEntity>>> GetListAsync(int skipCount, int maxCount, string sorting, string nameFilter)
    {
      var result = default(KeyValuePair<long, List<TaskEntity>>);
      try
      {
        result = await taskRepository.GetList(skipCount, maxCount, sorting, nameFilter);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<List<(TaskEntity, TriggerEntity)>> GetDueTasksAsync()
    {
      try
      {

        var now = DateTime.UtcNow;
        var tasks = await taskRepository.GetTasksToStartAsync(now);

        var result = new List<(TaskEntity, TriggerEntity)>();

        foreach (var task in tasks)
        {
          var nextRunTimeTrigger = await triggerDomainService.GetTaskNextRunTimeAsync(task);

          if (nextRunTimeTrigger.HasValue && nextRunTimeTrigger?.NextRunTime <= now)
          {
            var trigger = nextRunTimeTrigger.Value.Trigger;
            if (trigger != null)
            {
              await triggerDomainService.UpdateTriggerLastRunTime(trigger.Id, now);
            }
            result.Add((task, trigger));
          }
        }

        return result;
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }
    }

    public async Task<TaskEntity> CreateAsync(string name, string description)
    {
      try
      {
        var task = new TaskEntity(GuidGenerator.Create())
        {
          Name = name,
          IsActive = false,
          Description = description,
          CanRunManually = false,
          AllowForceStop = false,
          RestartAfterFailInterval = TimeSpan.FromMinutes(5),
          RestartAfterFailMaxAttempts = 3,
          Timeout = TimeSpan.FromMinutes(30),
          LastRunTimeUtc = null,
          Status = JobSchedulerTaskStatus.Inactive,
          NextRunTimeUtc = null,
        };

        task = await taskRepository.InsertAsync(task, true);

        return task;
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }
    }

    public async Task<bool> UpdateTask(TaskEntity task)
    {
      bool result = false;
      try
      {
        var trackedTask = await taskRepository.GetAsync(task.Id, true);

        if (trackedTask.Status == JobSchedulerTaskStatus.Running)
        {
          throw new UserFriendlyException("Cannot update task while it is running.", JobSchedulerErrorCodes.NowAllowedWhenTaskRunning);
        }

        if (task.IsActive)
        {
          if (trackedTask.Actions.Count == 0)
          {
            throw new UserFriendlyException("Cannot be activated while no actions was added", JobSchedulerErrorCodes.ActivateTaskNotAllowed);
          }
        }

        trackedTask.Name = task.Name;
        trackedTask.IsActive = task.IsActive;
        trackedTask.Description = task.Description;
        trackedTask.CanRunManually = task.CanRunManually;
        trackedTask.AllowForceStop = task.AllowForceStop;
        trackedTask.RestartAfterFailInterval = task.RestartAfterFailInterval;
        trackedTask.RestartAfterFailMaxAttempts = task.RestartAfterFailMaxAttempts;
        trackedTask.Timeout = task.Timeout;
        trackedTask.Status = task.IsActive ? JobSchedulerTaskStatus.Ready : JobSchedulerTaskStatus.Inactive;
        trackedTask.OnFailureRecepients = task.OnFailureRecepients;

        await triggerDomainService.UpdateTaskNextRunTimeAsync(trackedTask);

        await taskRepository.UpdateAsync(trackedTask, true);

        result = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
      try
      {
        await taskRepository.DeleteAsync(id, true);
        return true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }
    }
  }
}
