using Common.Module.Constants;
using EleonsoftModuleCollector.JobScheduler.Module.JobScheduler.Module.Domain.Helpers;
using Logging.Module;
using Messaging.Module.Messages;
using ModuleCollector.JobScheduler.Module.JobScheduler.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Validation;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.Repositories;

namespace VPortal.JobScheduler.Module.DomainServices
{
  public class TriggerDomainService : DomainService
  {
    private readonly IVportalLogger<TriggerDomainService> logger;
    private readonly ITriggerRepository triggerRepository;
    private readonly IDistributedEventBus messagePublisher;
    private readonly ITaskRepository _taskRepository;
    private readonly ITaskExecutionRepository _taskExecutionRepository;

    public TriggerDomainService(
        IVportalLogger<TriggerDomainService> logger,
        ITriggerRepository triggerRepository,
        IDistributedEventBus messagePublisher,
        ITaskRepository taskRepository,
        ITaskExecutionRepository taskExecutionRepository)
    {
      this.logger = logger;
      this.triggerRepository = triggerRepository;
      this.messagePublisher = messagePublisher;
      _taskRepository = taskRepository;
      _taskExecutionRepository = taskExecutionRepository;
    }

    public async Task<List<TriggerEntity>> GetListAsync(Guid? taskId, bool? isEnabledFilter = null)
    {
      try
      {
        var result = await triggerRepository.GetListAsync(taskId, isEnabledFilter);
        foreach (var item in result)
        {
          item.MapPersistentPropertiesToDisplayOnly();
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

    /// <summary>
    /// Calculates the next date and time the trigger should fire for a given last run time.
    /// </summary>
    /// <remarks>
    /// The result depends on the TimePeriodType specified in the entity.
    /// For the TimePeriodType.Monthly, if both the DaysOfMonth and DaysOfWeek are
    /// present in the entity, the DaysOfMonth will have the priority.
    /// </remarks>
    /// <param name="triggerId">The ID of the trigger.</param>
    /// <param name="lastRunTime">The time the trigger fired last time
    /// (better to provide the value that was calculated than the factual time of execution).
    /// </param>
    /// <returns>The next date and time the trigger should fire.</returns>


    public async Task<TriggerEntity> AddAsync(TriggerEntity trigger)
    {
      try
      {
        trigger.MapDisplayPropertiesToPersistent();
        ValidateTrigger(trigger);

        trigger.StartUtc = TriggerDateHelper.TrimToMinute(trigger.StartUtc);
        trigger.ExpireUtc = TriggerDateHelper.TrimToMinute(trigger.ExpireUtc);

        trigger.NextRunUtc = await GetTriggerNextRunTimeAsync(trigger, null);

        var entity = await triggerRepository.InsertAsync(trigger, true);
        var task = await _taskRepository.GetAsync(entity.TaskId);

        return entity;
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

    public async Task<TriggerEntity> GetByIdAsync(Guid id)
    {
      TriggerEntity result = null;
      try
      {
        var trigger = await triggerRepository.GetAsync(id);
        trigger.MapPersistentPropertiesToDisplayOnly();
        result = trigger;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<bool> SetTriggerIsEnabled(Guid triggerId, bool isEnabled)
    {
      bool result = false;
      try
      {
        var trackedEntity = await triggerRepository.GetAsync(triggerId, false);
        trackedEntity.IsEnabled = isEnabled;
        trackedEntity.NextRunUtc = await GetTriggerNextRunTimeAsync(trackedEntity, null);
        await triggerRepository.UpdateAsync(trackedEntity, true);
        var task = await _taskRepository.GetAsync(trackedEntity.TaskId);
        result = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<TriggerEntity> UpdateAsync(TriggerEntity trigger)
    {
      try
      {
        var trackedEntity = await triggerRepository.GetAsync(trigger.Id, true);
        trackedEntity.Name = trigger.Name;
        trackedEntity.StartUtc = TriggerDateHelper.TrimToMinute(trigger.StartUtc);
        trackedEntity.ExpireUtc = TriggerDateHelper.TrimToMinute(trigger.ExpireUtc);
        trackedEntity.PeriodType = trigger.PeriodType;
        trackedEntity.Period = trigger.Period;
        trackedEntity.DaysOfWeekList = trigger.DaysOfWeekList;
        trackedEntity.DaysOfWeekOccurencesList = trigger.DaysOfWeekOccurencesList;
        trackedEntity.DaysOfWeekOccurencesLast = trigger.DaysOfWeekOccurencesLast;
        trackedEntity.MonthsList = trigger.MonthsList;
        trackedEntity.DaysOfMonthList = trigger.DaysOfMonthList;
        trackedEntity.DaysOfMonthLast = trigger.DaysOfMonthLast;
        trackedEntity.RepeatTask = trigger.RepeatTask;
        trackedEntity.RepeatIntervalUnits = trigger.RepeatIntervalUnits;
        trackedEntity.RepeatIntervalUnitType = trigger.RepeatIntervalUnitType;
        trackedEntity.RepeatDurationUnits = trigger.RepeatDurationUnits;
        trackedEntity.RepeatDurationUnitType = trigger.RepeatDurationUnitType;
        trackedEntity.LastRun = null;
        trackedEntity.MapDisplayPropertiesToPersistent();

        ValidateTrigger(trackedEntity);

        trackedEntity.NextRunUtc = await GetTriggerNextRunTimeAsync(trackedEntity, null);

        await triggerRepository.UpdateAsync(trackedEntity, true);
        var task = await _taskRepository.GetAsync(trackedEntity.TaskId);

        return trackedEntity;
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

    public async Task UpdateTriggerLastRunTime(Guid triggerId, DateTime? lastRun)
    {
      try
      {
        var trackedEntity = await triggerRepository.GetAsync(triggerId, false);
        trackedEntity.LastRun = lastRun;
        await triggerRepository.UpdateAsync(trackedEntity, true);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
    }

    public async Task DeleteAsync(Guid id)
    {

      try
      {
        var trigger = await triggerRepository.GetAsync(id);
        await triggerRepository.DeleteAsync(id);
        var task = await _taskRepository.GetAsync(trigger.TaskId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
    }

    private void ValidateTrigger(TriggerEntity trigger)
    {
      ArgumentNullException.ThrowIfNull(trigger, nameof(trigger));

      if (trigger.TaskId == Guid.Empty)
        throw new AbpValidationException("TaskId cannot be empty.");

      if (string.IsNullOrWhiteSpace(trigger.Name))
        throw new AbpValidationException("Name is required.");

      // Start/Expire
      if (trigger.StartUtc == default)
        throw new AbpValidationException("StartUtc must be a valid date and time.");

      if (trigger.ExpireUtc.HasValue)
      {
        if (trigger.ExpireUtc.Value <= trigger.StartUtc)
          throw new AbpValidationException("ExpireUtc must be greater than StartUtc.");
      }

      TriggerDateHelper.ValidateTriggerDateProperties(trigger);
    }

    public async Task<DateTime?> GetTriggerNextRunTimeAsync(TriggerEntity trigger, DateTime? utcNow)
    {

      try
      {
        trigger.MapPersistentPropertiesToDisplayOnly();

        return TriggerDateHelper.GetNextRunTime(trigger, utcNow);
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

    /// <summary>
    /// If trigger has not value it means that this task will be retried after fail
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public async Task<(DateTime? NextRunTime, TriggerEntity? Trigger)?> GetTaskNextRunTimeAsync(TaskEntity task, List<TriggerEntity> triggers, DateTime? utcNow)
    {
      var lastExecution = await _taskExecutionRepository.GetNewestByFinishedAtAsync(task.Id);
      if (
          (lastExecution?.Status == JobSchedulerTaskExecutionStatus.Failed || lastExecution?.Status == JobSchedulerTaskExecutionStatus.Cancelled) &&
          lastExecution?.IsStatusChangedManually != true &&
          task.IsRetryEnabled &&
          task.CurrentRetryAttempt < task.RestartAfterFailMaxAttempts &&
          lastExecution?.FinishedAtUtc.HasValue == true &&
          task.RestartAfterFailInterval.HasValue)
      {
        return (lastExecution.FinishedAtUtc.Value.Add(task.RestartAfterFailInterval.Value), null);
      }

      foreach (var taskTrigger in triggers.Where(t => t.NextRunUtc is null))
      {
        taskTrigger.NextRunUtc = await GetTriggerNextRunTimeAsync(taskTrigger, utcNow);
      }

      var closestNextRunTime = triggers.Where(x => x.NextRunUtc != null).OrderBy(x => x.NextRunUtc).FirstOrDefault();

      return (closestNextRunTime?.NextRunUtc, closestNextRunTime);
    }

  public async Task<List<DateTime>> GetTriggerUpcomingRunTimes(Guid triggerId, DateTime? fromUtc, int count)
    {
      var result = new List<DateTime>();
      try
      {
        var trigger = await triggerRepository.GetAsync(triggerId);
        var nextRunTime = TriggerDateHelper.GetNextRunTime(trigger, fromUtc);
        while (nextRunTime.HasValue && result.Count < count)
        {
          result.Add(nextRunTime.Value);
          nextRunTime = TriggerDateHelper.GetNextRunTime(trigger, nextRunTime);
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }
      return result;
    }
  }
}

