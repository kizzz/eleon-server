using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Module.Constants;
using Eleon.AbpSdk.Lib.modules.HostExtensions.Module.Auth;
using Eleon.JobScheduler.Module.Eleon.JobScheduler.Module.Domain.Shared.DomainServices;
using Eleon.Logging.Lib.SystemLog.Contracts;
using EleonsoftAbp.Auth;
using EleonsoftModuleCollector.Commons.Module.Constants.BackgroundJobs;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedModule.modules.Helpers.Module;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.Repositories;

namespace VPortal.JobScheduler.Module.DomainServices
{
  public class TaskExecutionDomainService : DomainService
  {
    private static object runningDueTasksLocker = new object();

    private readonly IVportalLogger<TaskExecutionDomainService> logger;
    private readonly ICurrentUser currentUser;
    private readonly TaskDomainService taskDomainService;
    private readonly ICurrentTenant currentTenant;
    private readonly IDistributedEventBus _eventBus;
    private readonly TriggerDomainService _triggerDomainService;
    private readonly ITaskHubContext taskHubContext;
    private readonly ITaskRepository taskRepository;
    private readonly ITaskExecutionRepository taskExecutionRepository;
    private readonly IActionExecutionRepository actionExecutionRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IGuidGenerator _guidGenerator;

    public TaskExecutionDomainService(
      IVportalLogger<TaskExecutionDomainService> logger,
      ITaskRepository taskRepository,
      ITaskExecutionRepository taskExecutionRepository,
      IActionExecutionRepository actionExecutionRepository,
      ICurrentUser currentUser,
      TaskDomainService taskDomainService,
      ICurrentTenant currentTenant,
      IDistributedEventBus messagePublisher,
      TriggerDomainService triggerDomainService,
      IUnitOfWorkManager unitOfWorkManager,
      ITaskHubContext taskContext,
      IGuidGenerator guidGenerator = null
    )
    {
      this.logger = logger;
      this.taskRepository = taskRepository;
      this.taskExecutionRepository = taskExecutionRepository;
      this.actionExecutionRepository = actionExecutionRepository;
      this.currentUser = currentUser;
      this.taskDomainService = taskDomainService;
      this.currentTenant = currentTenant;
      this._eventBus = messagePublisher;
      this.taskHubContext = taskContext;
      _triggerDomainService = triggerDomainService;
      _unitOfWorkManager = unitOfWorkManager;
      _guidGenerator = guidGenerator ?? GuidGenerator; // Use injected or fall back to base class property
    }

    public async Task<TaskExecutionEntity> GetByIdAsync(Guid id)
    {
      try
      {
        var result = await taskExecutionRepository.GetAsync(id, true);
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

    public async Task<KeyValuePair<long, List<TaskExecutionEntity>>> GetTaskExecutionsListAsync(
      Guid taskId,
      int skipCount,
      int maxCount,
      string sorting
    )
    {
      var result = default(KeyValuePair<long, List<TaskExecutionEntity>>);
      try
      {
        result = await taskExecutionRepository.GetListAsync(taskId, skipCount, maxCount, sorting);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<bool> AcknowledgeActionCompletedAsync(
      Guid actionExecutionId,
      Guid taskExecutionId,
      JobSchedulerExecutionResult actionResult,
      string changedBy,
      bool manually
    )
    {
      changedBy ??= string.Empty;
      try
      {
        return await _unitOfWorkManager.ExecuteWithConcurrencyHandlingAsync(
          async uow =>
          {
            var actionExecutions = await actionExecutionRepository.GetListByTaskExecutionIdAsync(
              taskExecutionId
            );
            var actionExecution = actionExecutions.Single(x => x.Id == actionExecutionId);
            var taskExecution = await taskExecutionRepository.GetAsync(taskExecutionId, false);

            // Determine desired status
            var desiredStatus = actionResult switch
            {
              JobSchedulerExecutionResult.Success => JobSchedulerActionExecutionStatus.Completed,
              JobSchedulerExecutionResult.Cancelled => JobSchedulerActionExecutionStatus.Cancelled,
              _ => JobSchedulerActionExecutionStatus.Failed,
            };

            // Idempotency check: if already in desired final state, treat as success
            if (IdempotencyHelpers.IsStatusEquals(actionExecution.Status, desiredStatus))
            {
              logger.Log.LogWarning(
                "Action execution {ActionExecutionId} already in desired state ({Status}). Treating as idempotent success.",
                actionExecutionId,
                actionExecution.Status
              );
              var actionExecutionsAfterIdempotent =
                await actionExecutionRepository.GetListByTaskExecutionIdAsync(taskExecutionId);
              var anyExecutingActionAfterIdempotent = actionExecutionsAfterIdempotent.Any(x =>
                x.Status == JobSchedulerActionExecutionStatus.Executing
              );
              if (!anyExecutingActionAfterIdempotent)
              {
                await FinishTaskExecutionAsync(taskExecution, actionExecutionsAfterIdempotent);
              }
              await uow.CompleteAsync();
              return true;
            }

            // Only update if not already in a final state
            if (
              actionExecution.Status == JobSchedulerActionExecutionStatus.Completed
              || actionExecution.Status == JobSchedulerActionExecutionStatus.Failed
              || actionExecution.Status == JobSchedulerActionExecutionStatus.Cancelled
            )
            {
              logger.Log.LogWarning(
                "Action execution {ActionExecutionId} already in final state ({Status}) but different from desired ({DesiredStatus}).",
                actionExecutionId,
                actionExecution.Status,
                desiredStatus
              );
              await uow.CompleteAsync();
              return false; // Conflict - different outcome
            }

            // Update status only if not already in target state
            if (actionExecution.Status != desiredStatus)
            {
              actionExecution.Status = desiredStatus;
              actionExecution.CompletedAtUtc = DateTime.UtcNow;
              actionExecution.StatusChangedBy = changedBy;
              actionExecution.IsStatusChangedManually = manually;
              await actionExecutionRepository.UpdateAsync(actionExecution);
            }

            if (actionExecution.Status == JobSchedulerActionExecutionStatus.Completed)
            {
              await RequestNextActionExecution(
                taskExecution.Id,
                actionExecutions,
                actionExecution,
                taskExecution.Task
              );
            }

            var actionExecutionsAfterUpdate =
              await actionExecutionRepository.GetListByTaskExecutionIdAsync(taskExecutionId);
            bool anyExecutingAction = actionExecutionsAfterUpdate.Any(x =>
              x.Status == JobSchedulerActionExecutionStatus.Executing
            );

            if (!anyExecutingAction)
            {
              await FinishTaskExecutionAsync(taskExecution, actionExecutionsAfterUpdate);
            }

            await _eventBus.PublishAsync(
              new JobSchedulerActionExecutionCompletedMsg
              {
                ActionName = actionExecution.ActionName,
                EventName = actionExecution.EventName,
                ExecutionResult = actionResult,
              }
            );

            return true;
          },
          null,
          logger.Log,
          "AcknowledgeActionCompletedAsync",
          actionExecutionId
        );
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

    private async Task FinishTaskExecutionAsync(
      TaskExecutionEntity taskExecution,
      List<ActionExecutionEntity> actionExecutions
    )
    {
      try
      {
        // Idempotency check: if task execution is already in a final state, skip
        if (
          taskExecution.Status == JobSchedulerTaskExecutionStatus.Completed
          || taskExecution.Status == JobSchedulerTaskExecutionStatus.Cancelled
          || taskExecution.Status == JobSchedulerTaskExecutionStatus.Failed
        )
        {
          if (taskExecution.FinishedAtUtc.HasValue)
          {
            logger.Log.LogWarning(
              "Task execution {TaskExecutionId} already in final state ({Status}). Skipping finish operation.",
              taskExecution.Id,
              taskExecution.Status
            );
            return;
          }
        }

        var allCompleted = actionExecutions.All(x =>
          x.Status == JobSchedulerActionExecutionStatus.Completed
        );
        var anyCancelled = actionExecutions.Any(x =>
          x.Status == JobSchedulerActionExecutionStatus.Cancelled
        );

        JobSchedulerTaskExecutionStatus desiredStatus;
        if (allCompleted)
        {
          desiredStatus = JobSchedulerTaskExecutionStatus.Completed;
        }
        else if (anyCancelled)
        {
          desiredStatus = JobSchedulerTaskExecutionStatus.Cancelled;
        }
        else
        {
          desiredStatus = JobSchedulerTaskExecutionStatus.Failed;
        }

        // Only update if not already in desired state
        if (taskExecution.Status != desiredStatus)
        {
          taskExecution.Status = desiredStatus;
        }

        if (!taskExecution.FinishedAtUtc.HasValue)
        {
          taskExecution.FinishedAtUtc = DateTime.UtcNow;
        }

        await taskExecutionRepository.UpdateAsync(taskExecution);

        await _eventBus.PublishAsync(
          new JobSchedulerTaskExecutionCompletedMsg
          {
            ExecutionId = taskExecution.Id,
            ExecutionResult = taskExecution.Status,
          }
        );

        var task = await taskRepository.GetWithTriggerAsync(taskExecution.TaskId);
        if (task.Status == JobSchedulerTaskStatus.Running)
        {
          task.Status = JobSchedulerTaskStatus.Ready;
          await taskRepository.UpdateAsync(task);
        }

        if (taskExecution.Status == JobSchedulerTaskExecutionStatus.Failed)
        {
          if (!string.IsNullOrWhiteSpace(task.OnFailureRecepients))
          {
            await _eventBus.PublishAsync(
              new AddNotificationMsg
              {
                Notification = new EleonsoftNotification
                {
                  Recipients = task
                    .OnFailureRecepients.Split(";")
                    .Select(x => new RecipientEto
                    {
                      RecipientAddress = x,
                      Type = NotificatorRecepientType.Direct,
                    })
                    .ToList(),
                  Type = new SystemNotificationType { LogLevel = SystemLogLevel.Critical },
                  Message =
                    $"Task {task.Name} failed. \nFailed Actions:\n{string.Join('\n', actionExecutions.Where(x => x.Status == JobSchedulerActionExecutionStatus.Failed).Select(x => $"> {x.ActionName}"))}",
                },
              }
            );
          }
        }

        await taskHubContext.TaskCompleted(task);
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

    public async Task<bool> RequestTaskExecutionAsync(
      Guid taskId,
      bool manual,
      TriggerEntity runnedTrigger
    )
    {
      try
      {
        var task = await taskRepository.GetAsync(taskId, true);

        if (manual && !task.CanRunManually)
        {
          throw new UserFriendlyException(
            "Run this task mannually not allowed",
            JobSchedulerErrorCodes.RunManuallyNotAllowed
          );
        }

        if (task.Status == JobSchedulerTaskStatus.Running && task.AllowForceStop)
        {
          logger.Log.LogWarning($"Task {task.Id} is forced to stop due to next scheduled run");
          await StopTaskExecutionAsync(taskId, false);
          task = await taskRepository.GetAsync(taskId, true); // take again after stopping
        }

        if (task.Status != JobSchedulerTaskStatus.Ready)
        {
          throw new UserFriendlyException(
            "Task is not ready",
            JobSchedulerErrorCodes.CanOnlyRunReadyTask
          );
        }

        // Validate that task has actions before execution
        if (task.Actions == null || !task.Actions.Any())
        {
          throw new UserFriendlyException(
            "Task must have at least one action before execution",
            JobSchedulerErrorCodes.TaskMustHaveActions
          );
        }

        // Determine if this is a retry scenario
        // Retry happens when: last execution failed/cancelled, retry is enabled, not manually changed, and FinishedAtUtc is set
        var lastExecution = task.Executions.OrderByDescending(x => x.FinishedAtUtc ?? DateTime.MinValue).FirstOrDefault();
        bool isRetryScenario = lastExecution != null &&
          (lastExecution.Status == JobSchedulerTaskExecutionStatus.Failed ||
           lastExecution.Status == JobSchedulerTaskExecutionStatus.Cancelled) &&
          lastExecution.IsStatusChangedManually != true &&
          lastExecution.FinishedAtUtc.HasValue && // FinishedAtUtc must be set for retry
          task.IsRetryEnabled &&
          task.CurrentRetryAttempt < task.RestartAfterFailMaxAttempts;

        if (manual || runnedTrigger != null)
        {
          // Reset retry attempt for manual runs or scheduled triggers (not retries)
          task.CurrentRetryAttempt = 0;
        }
        else if (isRetryScenario)
        {
          // Only increment retry attempt when actually retrying after a failure
          task.CurrentRetryAttempt++;
        }
        // If not a retry scenario and not manual/trigger, keep CurrentRetryAttempt as is (should be 0 for first runs)

        var taskExecution =
          await CreateTaskExecutionAsync(task, runnedTrigger)
          ?? throw new Exception("Unable to create an instance of TaskExecution");

        // Idempotency: check if already in Running status
        if (task.Status == JobSchedulerTaskStatus.Running)
        {
          logger.Log.LogInformation(
            "Task {TaskId} already in Running status. Treating as idempotent success.",
            task.Id
          );
        }
        else
        {
          task.LastRunTimeUtc = DateTime.UtcNow;
          task.Status = JobSchedulerTaskStatus.Running;
          try
          {
            await taskRepository.UpdateAsync(task, true);
          }
          catch (AbpDbConcurrencyException ex)
          {
            logger.Log.LogWarning(
              ex,
              "Concurrency conflict while updating task {TaskId} to Running status. Waiting for desired state...",
              task.Id
            );

            await ConcurrencyExtensions.WaitForDesiredStateAsync(
              async () =>
              {
                var currentTask = await taskRepository.GetAsync(task.Id);
                var isDesired = currentTask.Status == JobSchedulerTaskStatus.Running;
                var details = $"Status={currentTask.Status}";
                return new ConcurrencyExtensions.ConcurrencyWaitResult<TaskEntity>(isDesired, currentTask, details);
              },
              logger.Log,
              "RequestTaskExecutionAsync",
              task.Id
            );
          }
        }
        if (taskExecution.ActionExecutions != null && taskExecution.ActionExecutions.Any())
        {
          await RequestNextActionExecution(
            taskExecution.Id,
            taskExecution.ActionExecutions.ToList(),
            null,
            task
          );
        }
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

    private async Task<bool> RequestNextActionExecution(
      Guid taskExecutionId,
      List<ActionExecutionEntity> actionExecutions,
      ActionExecutionEntity? actionExecutionEntity,
      TaskEntity taskEntity
    )
    {

      try
      {
        var actionExecutionsToStart = actionExecutions
          .Where(x => x.Status == JobSchedulerActionExecutionStatus.NotStarted)
          .ToList();

        foreach (var actionExecutionToStart in actionExecutionsToStart)
        {
          var parents = actionExecutions.Where(x =>
            actionExecutionToStart.ParentActionExecutions.Any(p =>
              p.ParentActionExecutionId == x.Id
            )
          );
          if (parents.Any(x => x.Status != JobSchedulerActionExecutionStatus.Completed))
          {
            continue;
          }

          var parentJobs = parents.Where(x => x.JobId.HasValue).Select(x => x.JobId.Value).ToList();
          await ExecuteActionAsync(actionExecutionToStart, parentJobs, taskEntity, taskExecutionId);
        }

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

    private async Task<bool> ExecuteActionAsync(
      ActionExecutionEntity actionExecution,
      List<Guid> parentJobsIds,
      TaskEntity taskEntity,
      Guid taskExecutionEntityId
    )
    {
      bool result = false;
      try
      {
        actionExecution = await actionExecutionRepository.GetAsync(actionExecution.Id); // Ensure that actionEntity was loaded

        // Ensure Action property is set - required for accessing Action properties
        if (actionExecution.Action == null && actionExecution.ActionId.HasValue)
        {
          // Try to find the action from the task entity
          var action = taskEntity.Actions.FirstOrDefault(a => a.Id == actionExecution.ActionId.Value);
          if (action != null)
          {
            actionExecution.Action = action;
          }
        }

        if (actionExecution.Action == null)
        {
          logger.Log.LogError($"Action execution {actionExecution.Id} does not have Action property set. ActionId: {actionExecution.ActionId}");
          return false;
        }

        if (actionExecution.JobId.HasValue)
        {
          var retryJobMessage = new RetryBackgroundJobMsg
          {
            JobId = actionExecution.JobId.Value,
            StartExecutionParams = actionExecution.ActionParams,
            StartExecutionExtraParams = actionExecution.Id.ToString(),
            TenantId = this.currentTenant.Id,
            TenantName = this.currentTenant.Name,
            TimeoutInMinutes = actionExecution.Action.TimeoutInMinutes,
            MaxRetryAttempts = actionExecution.Action.MaxRetryAttempts,
            RetryInMinutes = ((int?)actionExecution.Action.RetryInterval?.TotalMinutes) ?? -1,
            OnFailureRecepients = actionExecution.Action.OnFailureRecepients,
          };
          await _eventBus.PublishAsync(retryJobMessage);
        }
        else
        {
          var jobId = _guidGenerator.Create();
          var addBackgroundJobMsg = new CreateBackgroundJobMsg
          {
            Id = jobId,
            StartExecutionParams = actionExecution.ActionParams,
            ParentJobsIds = parentJobsIds,
            Type = actionExecution.EventName,
            Initiator = $"Task::{taskEntity.Name}",
            ScheduleExecutionDateUtc = DateTime.UtcNow,
            IsRetryAllowed = true,
            StartExecutionExtraParams = actionExecution.Id.ToString(),
            SourceId = "JobScheduler",
            SourceType = BackgroundJobConstants.SourceType.SystemModule,
            IsSystemInternal = false,
            TenantId = this.currentTenant.Id,
            TenantName = this.currentTenant.Name,
            TimeoutInMinutes = actionExecution.Action.TimeoutInMinutes,
            MaxRetryAttempts = actionExecution.Action.MaxRetryAttempts,
            RetryInMinutes = ((int?)actionExecution.Action.RetryInterval?.TotalMinutes) ?? -1,
            OnFailureRecepients = actionExecution.Action.OnFailureRecepients,
          };

          var extraProperties = new Dictionary<string, string>();
          extraProperties.Add("ActionId", actionExecution.ActionId.ToString());
          extraProperties.Add("ActionExecutionId", actionExecution.Id.ToString());
          extraProperties.Add("TaskExecutionId", taskExecutionEntityId.ToString());
          extraProperties.Add("TaskId", taskEntity.Id.ToString());
          addBackgroundJobMsg.ExtraProperties = extraProperties;

          await _eventBus.PublishAsync(addBackgroundJobMsg);
          actionExecution.JobId = jobId;
        }

        actionExecution.Status = JobSchedulerActionExecutionStatus.Executing;
        actionExecution.StartedAtUtc = DateTime.UtcNow;
        await actionExecutionRepository.UpdateAsync(actionExecution);

        await _eventBus.PublishAsync(new JobSchedulerActionExecutionAddedMsg());
        result = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    private async Task<TaskExecutionEntity> CreateTaskExecutionAsync(
      TaskEntity task,
      TriggerEntity runnedTrigger
    )
    {
      try
      {
        var previousExecution = task
          .Executions.OrderByDescending(x => x.StartedAtUtc)
          .FirstOrDefault();
        if (
          previousExecution != null
          && (
            previousExecution.ActionExecutions == null
            || previousExecution.ActionExecutions.Count == 0
          )
        )
        {
          previousExecution = await taskExecutionRepository.GetAsync(previousExecution.Id, true);
        }

        var taskExecutionId = _guidGenerator.Create();
        var actionExecutions = task.Actions.ToDictionary(
          x => x.Id,
          x => new ActionExecutionEntity(_guidGenerator.Create(), taskExecutionId)
          {
            ActionId = x.Id,
            ParentActionExecutions = [],
            ActionParams = x.ActionParams,
            ActionExtraParams = x.ActionExtraParams,
            EventName = x.EventName,
            ActionName = x.DisplayName,
            Status = JobSchedulerActionExecutionStatus.NotStarted,
            // NOTE: JobId is intentionally set to null for new action executions.
            // When an action execution is retried (via ExecuteActionAsync), if it already has a JobId,
            // the system publishes a RetryBackgroundJobMsg instead of creating a new job.
            // Reusing JobId from previous execution is not needed because:
            // 1. Each task execution creates fresh action executions
            // 2. Retry logic is handled at the action execution level, not task execution level
            // 3. If an action needs retry, its JobId is preserved in the existing ActionExecutionEntity
            JobId = null,
          }
        );

        foreach (var action in task.Actions) // Populate parent-child relationships
        {
          var execution = actionExecutions[action.Id];
          execution.ParentActionExecutions = action
            .ParentActions.Select(x => new ActionExecutionParentEntity(
              execution.Id,
              actionExecutions[x.ParentActionId].Id
            ))
            .ToList();
        }

        var taskExecution = new TaskExecutionEntity(taskExecutionId, task.Id)
        {
          Status = JobSchedulerTaskExecutionStatus.Executing,
          StartedAtUtc = DateTime.UtcNow,
          RunnedByUserId = currentUser?.Id,
          RunnedByUserName = currentUser?.Name,
          RunnedByTriggerId = runnedTrigger?.Id,
          RunnedByTriggerName = runnedTrigger?.Name,
          ActionExecutions = actionExecutions.Select(x => x.Value).ToList(),
        };
        taskExecution = await taskExecutionRepository.InsertAsync(taskExecution, true);
        return taskExecution;
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

    public async Task<bool> StopTaskExecutionAsync(Guid taskId, bool manually)
    {

      try
      {
        var now = DateTime.UtcNow;
        var task = await taskRepository.GetAsync(taskId, true);

        if (task.Status != JobSchedulerTaskStatus.Running)
        {
          throw new UserFriendlyException(
            "Task is not running",
            JobSchedulerErrorCodes.CanOnlyStopRunningTask
          );
        }

        var taskExecutions = await taskExecutionRepository.GetListAsync(
          taskId,
          0,
          int.MaxValue,
          null
        );

        if (taskExecutions.Value != null)
        {
          foreach (var taskExecution in taskExecutions.Value)
        {
          if (taskExecution.Status == JobSchedulerTaskExecutionStatus.Executing)
          {
            taskExecution.Status = JobSchedulerTaskExecutionStatus.Cancelled;
            taskExecution.FinishedAtUtc = now;
            await taskExecutionRepository.UpdateAsync(taskExecution);
            await _eventBus.PublishAsync(
              new JobSchedulerTaskExecutionCanceledMsg
              {
                TaskExecutionId = taskExecution.Id,
                TaskId = taskExecution.TaskId,
              }
            );

            if (taskExecution.ActionExecutions != null)
            {
              foreach (var action in taskExecution.ActionExecutions)
              {
                if (
                  action.Status == JobSchedulerActionExecutionStatus.Executing
                  || action.Status == JobSchedulerActionExecutionStatus.NotStarted
                )
                {
                  if (action.JobId.HasValue)
                  {
                     await _eventBus.PublishAsync(
                       new CancelBackgroundJobMsg
                       {
                         JobId = action.JobId.Value,
                         TenantId = this.currentTenant.Id,
                         TenantName = this.currentTenant.Name,
                         IsManually = true,
                         CancelledMessage = manually
                           ? $"Cancelled due to task stop by user with user name: {this.currentUser.UserName} and user id: {this.currentUser.Id}."
                           : "Cancelled due to task forced to stop by next schedule run.",
                       }
                     );
                  }
                  else
                  {
                    action.Status = JobSchedulerActionExecutionStatus.Cancelled;
                    action.CompletedAtUtc = now;
                    action.StatusChangedBy = manually ? this.currentUser.GetName() : "JobScheduler";
                    action.IsStatusChangedManually = manually;
                    await actionExecutionRepository.UpdateAsync(action);
                  }
                }
              }
            }
          }
        }
        }

        // Idempotency: check if already in Ready status
        if (task.Status == JobSchedulerTaskStatus.Ready)
        {
          logger.Log.LogInformation(
            "Task {TaskId} already in Ready status. Treating as idempotent success.",
            task.Id
          );
        }
        else
        {
          task.Status = JobSchedulerTaskStatus.Ready;
          try
          {
            await taskRepository.UpdateAsync(task, true);
          }
          catch (AbpDbConcurrencyException ex)
          {
            logger.Log.LogWarning(
              ex,
              "Concurrency conflict while updating task {TaskId} to Ready status. Waiting for desired state...",
              task.Id
            );

            await ConcurrencyExtensions.WaitForDesiredStateAsync(
              async () =>
              {
                var currentTask = await taskRepository.GetAsync(task.Id);
                var isDesired = currentTask.Status == JobSchedulerTaskStatus.Ready;
                var details = $"Status={currentTask.Status}";
                return new ConcurrencyExtensions.ConcurrencyWaitResult<TaskEntity>(isDesired, currentTask, details);
              },
              logger.Log,
              "StopTaskExecutionAsync",
              task.Id
            );
          }
        }
        return true;
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

    public async Task<List<ActionExecutionEntity>> GetRunningExecutionByJobIdAsync(Guid jobId)
    {

      try
      {
        var dbSet = await actionExecutionRepository.GetDbSetAsync();

        var result = await dbSet
          .AsNoTracking()
          .Where(x => x.Status == JobSchedulerActionExecutionStatus.Executing)
          .Where(x => x.JobId == jobId)
          .ToListAsync();

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
  }
}
