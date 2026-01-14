using Common.Module.Constants;
using Eleon.BackgroundJobs.Module.Eleon.BackgroundJobs.Module.Domain.Shared.DomainServices;
using Eleon.Logging.Lib.SystemLog.Contracts;
using EleonsoftModuleCollector.BackgroundJobs.Module.BackgroundJobs.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedModule.modules.Helpers.Module;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.Repositories;

namespace VPortal.BackgroundJobs.Module.DomainServices
{
  public class BackgroundJobDomainService : DomainService, IBackgroundJobDomainService
  {
    private readonly IVportalLogger<BackgroundJobDomainService> logger;
    private readonly IBackgroundJobExecutionsRepository backgroundJobExecutionsRepository;
    private readonly IBackgroundJobsRepository backgroundJobsRepository;
    private readonly IBackgroundJobHubContext backgroundJobHubContext;
    private readonly IUnitOfWorkManager unitOfWorkManager;
    private readonly IDistributedEventBus _eventBus;
    private readonly IConfiguration configuration;
    private readonly IObjectMapper<ModuleDomainModule> objectMapper;

    public BackgroundJobDomainService(
        IVportalLogger<BackgroundJobDomainService> logger,
        IBackgroundJobExecutionsRepository backgroundJobExecutionsRepository,
        IBackgroundJobsRepository backgroundJobsRepository,
        IBackgroundJobHubContext backgroundJobHubContext,
        IUnitOfWorkManager unitOfWorkManager,
        IDistributedEventBus massTransitPublisher,
        IConfiguration configuration,
        IObjectMapper<ModuleDomainModule> objectMapper)
    {
      this.logger = logger;
      this.backgroundJobExecutionsRepository = backgroundJobExecutionsRepository;
      this.backgroundJobsRepository = backgroundJobsRepository;
      this.unitOfWorkManager = unitOfWorkManager;
      this._eventBus = massTransitPublisher;
      this.configuration = configuration;
      this.backgroundJobHubContext = backgroundJobHubContext;
      this.objectMapper = objectMapper;
    }

    /// <summary>
    /// Fetches executable jobs by DateTime.Now.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<List<BackgroundJobEntity>> GetCurrentJobsAsync()
    {
      try
      {
        var result = await backgroundJobsRepository.GetByDateTime(DateTime.UtcNow);
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

    public async Task<List<BackgroundJobEntity>> GetRetryJobsAsync()
    {
      try
      {
        var result = await backgroundJobsRepository.GetRetryJobsAsync(DateTime.UtcNow);
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

    public async Task<List<BackgroundJobEntity>> GetByType(string type)
    {
      try
      {
        var result = await backgroundJobsRepository.GetByType(type);
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

    public async Task<BackgroundJobEntity> GetAsync(Guid id)
    {
      BackgroundJobEntity result = null;
      try
      {
        result = await backgroundJobsRepository.GetAsync(id);
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

    public async Task<BackgroundJobEntity> CreateAsync(
        Guid? tenantId,
        Guid jobId,
        Guid? parentJobId,
        string type,
        string initiator,
        bool isRetryAllowed,
        string description,
        string startExecutionParams,
        DateTime scheduleExecutionDate,
        bool isSystemInternal,
        string startExecutionExtraParams,
        string sourceId,
        string sourceType,
        int timeoutInMinutes,
        int maxRetryAttempts,
        int retryInMinutes,
        string onFailureRecepients,
        Dictionary<string, string> extraProperties = null)
    {
      startExecutionParams ??= string.Empty;
      startExecutionExtraParams ??= string.Empty;
      onFailureRecepients ??= string.Empty;

      BackgroundJobEntity result = null;
      try
      {
        using (CurrentTenant.Change(tenantId))
        {
          result = new BackgroundJobEntity(jobId)
          {
            TenantId = tenantId, // Explicitly set TenantId for IMultiTenant entity
            Type = type,
            ParentJobId = parentJobId,
            IsRetryAllowed = isRetryAllowed,
            Initiator = initiator,
            Description = description,
            StartExecutionParams = startExecutionParams,
            LastExecutionDateUtc = DateTime.UtcNow,
            Status = BackgroundJobStatus.New,
            ScheduleExecutionDateUtc = scheduleExecutionDate,
            EnvironmentId = configuration.GetValue<string>("EnvironmentId", string.Empty) ?? string.Empty,
            IsSystemInternal = isSystemInternal,
            StartExecutionExtraParams = startExecutionExtraParams,
            SourceId = sourceId,
            SourceType = sourceType,
            TimeoutInMinutes = timeoutInMinutes,
            MaxRetryAttempts = maxRetryAttempts,
            RetryIntervalInMinutes = retryInMinutes,
            OnFailureRecepients = onFailureRecepients
          };
          if (extraProperties != null)
          {
            foreach (var prop in extraProperties)
            {
              result.SetProperty(prop.Key, prop.Value);
            }
          }

          using (var unitOfWork = unitOfWorkManager.Begin(unitOfWorkManager.Current != null))
          {
            await backgroundJobsRepository.InsertAsync(result, autoSave: true);
            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CompleteAsync();
          }
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

    public async Task<BackgroundJobExecutionEntity> StartExecutionAsync(Guid backgroundJobId, bool isManualRetry = false, bool autoRetry = false)
    {
      try
      {
        using var unitOfWork = unitOfWorkManager.Begin(true);
        var job = await backgroundJobsRepository.GetAsync(backgroundJobId, true);

        job.NextRetryTimeUtc = null;
        if (!autoRetry)
        {
          job.CurrentRetryAttempt = 0;
        }

        if (isManualRetry || autoRetry)
        {
          if (isManualRetry && !job.IsRetryAllowed)
          {
            throw new UserFriendlyException("Retry not allowed for this job");
          }

          if (autoRetry)
          {
            job.CurrentRetryAttempt++;
          }

          job.Status = BackgroundJobStatus.Retring;
        }
        else
        {
          if (job.Status == BackgroundJobStatus.Completed || job.Status == BackgroundJobStatus.Cancelled)
          {
            throw new UserFriendlyException("Start finished job not allowed");
          }
        }

        if (job.Status == BackgroundJobStatus.Executing && job.Executions.Any(x => x.Status == BackgroundJobExecutionStatus.Starting || x.Status == BackgroundJobExecutionStatus.Started))
        {
          throw new UserFriendlyException("Start executing job not allowed");
        }

        var dateTimeNow = DateTime.UtcNow;

        var execution = new BackgroundJobExecutionEntity(GuidGenerator.Create())
        {
          BackgroundJobEntityId = backgroundJobId,
          ExecutionStartTimeUtc = dateTimeNow,
          ExecutionEndTimeUtc = null,
          Status = BackgroundJobExecutionStatus.Starting,
          IsRetryExecution = job.Status == BackgroundJobStatus.Errored,
          StartExecutionParams = job.StartExecutionParams,
          StartExecutionExtraParams = job.StartExecutionExtraParams,
          Messages = new List<BackgroundJobMessageEntity>()
                    {
                        new BackgroundJobMessageEntity(Guid.NewGuid())
                        {
                            MessageType = BackgroundJobMessageType.Info,
                            TextMessage = $"{job} execution starting",
                            CreationTime = DateTime.UtcNow
                        }
                    }
        };

        job.Executions.Add(execution);
        job.LastExecutionDateUtc = DateTime.UtcNow;
        job.UpdateStatus(BackgroundJobStatus.Executing);

        await backgroundJobsRepository.UpdateAsync(job);

        if (job.IsSystemInternal)
        {
          await _eventBus.PublishAsync(new StartingInternalSystemJobExecutionMsg
          {
            BackgroundJob = objectMapper.Map<BackgroundJobEntity, BackgroundJobEto>(job),
            Execution = objectMapper.Map<BackgroundJobExecutionEntity, BackgroundJobExecutionEto>(execution),
            TenantId = job.TenantId,
          });
        }
        else
        {
          await _eventBus.PublishAsync(new StartingJobExecutionMsg
          {
            BackgroundJob = objectMapper.Map<BackgroundJobEntity, BackgroundJobEto>(job),
            Execution = objectMapper.Map<BackgroundJobExecutionEntity, BackgroundJobExecutionEto>(execution),
            TenantId = job.TenantId,
          });
        }

        await unitOfWork.SaveChangesAsync();
        await unitOfWork.CompleteAsync();

        return execution;
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

    public async Task CancelJobAsync(Guid jobId, string cancelledBy, bool manually, string cancelledMessage)
    {
      cancelledBy ??= string.Empty;
      try
      {
        using var unitOfWork = unitOfWorkManager.Begin(true);
        var job = await backgroundJobsRepository.GetAsync(jobId, true);
        var dateTimeNow = DateTime.UtcNow;

        if (job.Status == BackgroundJobStatus.Cancelled || job.Status == BackgroundJobStatus.Completed || job.Status == BackgroundJobStatus.Errored)
        {
          throw new UserFriendlyException("Cancel finished background job not allowed");
        }

        foreach (var execution in job.Executions)
        {
          if (execution == null)
          {
            Logger.LogError("Execution was not found for job {JobId} with executionId {ExecutionId}", jobId, execution.Id);
            continue;
          }

          if (execution.Status == BackgroundJobExecutionStatus.Cancelled || execution.Status == BackgroundJobExecutionStatus.Completed || execution.Status == BackgroundJobExecutionStatus.Errored)
          {
            // ignored
          }
          else
          {
            execution.UpdateStatus(BackgroundJobExecutionStatus.Cancelled);
            execution.StatusChangedBy = cancelledBy;
            execution.IsStatusChangedManually = manually;

            if (string.IsNullOrWhiteSpace(cancelledMessage))
            {
              throw new Exception("Cancelled job without cancelled message");
            }
            logger.Log.LogWarning($"Backgroundjob with id: {jobId} cancelled with message: \"{cancelledMessage}\"");
            if (execution.Status == BackgroundJobExecutionStatus.Cancelled)
            {
              execution.AddMessage(BackgroundJobMessageType.Warn, cancelledMessage);
            }
          }
        }

        job.UpdateStatus(BackgroundJobStatus.Cancelled);
        job.JobFinishedUtc = dateTimeNow;

        var autoRetry = InternalRetry(job, job.Executions.OrderByDescending(x => x.ExecutionEndTimeUtc == null).ThenByDescending(x => x.ExecutionEndTimeUtc).FirstOrDefault());

        await backgroundJobsRepository.UpdateAsync(job);

        if (!autoRetry)
        {
          if (job.IsSystemInternal)
          {
            var jobAckedMsg = new BackgroundJobCompletedMsg
            {
              BackgroundJob = objectMapper.Map<BackgroundJobEntity, BackgroundJobEto>(job),
              JobId = job.Id,
              CompletedBy = cancelledBy,
              IsManually = manually,
              CompletionStatus = job.Status,
              TenantId = job.TenantId,
              TenantName = CurrentTenant.Name
            };
            await _eventBus.PublishAsync(jobAckedMsg);
          }
          else
          {
            var jobAckedMsg = new BackgroundJobCompletedMsg
            {
              BackgroundJob = objectMapper.Map<BackgroundJobEntity, BackgroundJobEto>(job),
              JobId = job.Id,
              CompletedBy = cancelledBy,
              IsManually = manually,
              CompletionStatus = job.Status,
              TenantId = job.TenantId,
              TenantName = CurrentTenant.Name
            };
            await _eventBus.PublishAsync(jobAckedMsg);
          }
        }

        await backgroundJobsRepository.UpdateAsync(job);

        await unitOfWork.SaveChangesAsync();
        await unitOfWork.CompleteAsync();
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

    public async Task<BackgroundJobExecutionEntity> MarkExecutionStartedAsync(Guid jobId, Guid executionId)
    {
      try
      {
        return await unitOfWorkManager.ExecuteWithConcurrencyHandlingAsync(
          async _ =>
          {
            var job = await backgroundJobsRepository.GetAsync(jobId, true);
            var execution = job.Executions.FirstOrDefault(x => x.Id == executionId);

            if (execution == null)
            {
              Logger.LogError("Execution was not found for job {JobId} with executionId {ExecutionId}", jobId, executionId);
              throw new EntityNotFoundException(typeof(BackgroundJobExecutionEntity), executionId);
            }

            if (execution.Status == BackgroundJobExecutionStatus.Starting)
            {
              execution.UpdateStatus(BackgroundJobExecutionStatus.Started);
            }

            if (execution.Status != BackgroundJobExecutionStatus.Started)
            {
              execution.Messages.Add(new BackgroundJobMessageEntity(Guid.NewGuid())
              {
                MessageType = BackgroundJobMessageType.Error,
                TextMessage = $"Failed to update execution status from {execution.Status} to {BackgroundJobExecutionStatus.Started}"
              });
            }
            else
            {
              execution.AddMessage(BackgroundJobMessageType.Info, $"{job} execution started");
            }

            await backgroundJobsRepository.UpdateAsync(job);
            return execution;
          },
          async () =>
          {
            // Reload and verify state
            using var verifyUow = unitOfWorkManager.Begin(false);
            var currentJob = await backgroundJobsRepository.FindAsync(jobId, includeDetails: true);

            if (currentJob == null)
            {
              logger.Log.LogWarning("Job {JobId} no longer exists while verifying concurrency conflict.", jobId);
              throw new EntityNotFoundException(typeof(BackgroundJobEntity), jobId);
            }

            var currentExecution = currentJob.Executions.FirstOrDefault(x => x.Id == executionId);
            if (currentExecution == null)
            {
              logger.Log.LogWarning("Execution {ExecutionId} no longer exists on job {JobId} while verifying concurrency conflict.",
                  executionId, jobId);
              throw new EntityNotFoundException(typeof(BackgroundJobExecutionEntity), executionId);
            }

            if (currentExecution.Status == BackgroundJobExecutionStatus.Started)
            {
              return false;
            }

            if (IsFinalExecutionStatus(currentExecution.Status))
            {
              logger.Log.LogWarning(
                "Execution {ExecutionId} for job {JobId} already reached final state ({Status}) while marking started. Treating as success.",
                executionId, jobId, currentExecution.Status);
              return false;
            }

            return true;
          },
          logger.Log,
          "MarkExecutionStartedAsync",
          executionId
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

    public async Task<BackgroundJobExecutionEntity> CompleteExecutionAsync(
        Guid jobId,
        Guid executionId,
        bool successfully,
        string retryExecutionParams,
        string retryExecutionExtraParams,
        List<BackgroundJobMessageEntity> messages,
        string result,
        string completedBy,
        bool manually)
    {
      retryExecutionParams ??= string.Empty;
      retryExecutionExtraParams ??= string.Empty;
      messages ??= new List<BackgroundJobMessageEntity>();
      result ??= string.Empty;
      completedBy ??= string.Empty;
      try
      {
        return await unitOfWorkManager.ExecuteWithConcurrencyHandlingAsync(
          async _ =>
          {
            var job = await backgroundJobsRepository.GetAsync(jobId, true);
            var dateTimeNow = DateTime.UtcNow;

            var execution = job.Executions.FirstOrDefault(x => x.Id == executionId);
            if (execution == null)
            {
              Logger.LogError("Execution was not found for job {JobId} with executionId {ExecutionId}", jobId, executionId);
              return null;
            }

            // Idempotency check: if already in desired final state, treat as success
            if (IsFinalWithSameOutcome(execution.Status, successfully))
            {
              logger.Log.LogWarning(
                  "Execution {ExecutionId} for job {JobId} already in final state ({Status}) matching desired outcome (successfully={Successfully}). Treating as idempotent success.",
                  executionId, jobId, execution.Status, successfully);

              // If job is also final, we can skip the entire operation
              if (IsJobFinal(job))
              {
                return execution;
              }
              // Otherwise, we may still need to update job status, so continue
            }

            // Fix: Don't transition Completed -> Errored on duplicate completion
            // Prevent transitioning from one final state to another
            if (successfully)
            {
              // Only update if not already completed
              if (execution.Status != BackgroundJobExecutionStatus.Completed)
              {
                execution.UpdateStatus(BackgroundJobExecutionStatus.Completed);
              }
            }
            else
            {
              // Only update if not already errored AND not already completed
              // Prevent transitioning from Completed to Errored
              if (execution.Status != BackgroundJobExecutionStatus.Errored &&
                  execution.Status != BackgroundJobExecutionStatus.Completed)
              {
                execution.UpdateStatus(BackgroundJobExecutionStatus.Errored);
              }
            }

            foreach (var message in messages)
            {
              execution.AddMessage(message.MessageType, message.TextMessage, message.CreationTime);
            }

            execution.StatusChangedBy = completedBy;
            execution.IsStatusChangedManually = manually;

            if (execution.Status == BackgroundJobExecutionStatus.Errored || !successfully)
            {
              if (messages.Count == 0)
              {
                execution.AddMessage(BackgroundJobMessageType.Error, successfully ? "Execution was already completed" : "Execution errored");
              }

              job.StartExecutionParams = retryExecutionParams;
              job.StartExecutionExtraParams = retryExecutionExtraParams;
              job.UpdateStatus(BackgroundJobStatus.Errored);
              if (!string.IsNullOrWhiteSpace(job.OnFailureRecepients))
              {
                await _eventBus.PublishAsync(new AddNotificationMsg
                {
                  Notification = new EleonsoftNotification
                  {
                    Recipients = job.OnFailureRecepients
                        .Split(BackgroundJobsConstants.RecepientsSeparator)
                        .Select(x => new RecipientEto { RecipientAddress = x, Type = NotificatorRecepientType.Direct })
                        .ToList(),
                    Type = new SystemNotificationType { LogLevel = SystemLogLevel.Critical },
                    Message = $"Background job {job.Type} failed. \nLogs:\n{string.Join('\n', execution.Messages.Select(x => $"[{x.MessageType.ToString().ToUpper()} {x.CreationTime}] {x.TextMessage}"))}",
                  }
                });
              }

              logger.Log.LogError($"Background job {job.Type} {job.Id} failed.");
            }
            else
            {
              execution.AddMessage(BackgroundJobMessageType.Info, $"{job} completed");
              job.UpdateStatus(BackgroundJobStatus.Completed);
            }

            job.Result = result;
            job.JobFinishedUtc = dateTimeNow;

            var autoRetry = InternalRetry(job, execution);

            if (!autoRetry)
            {
              if (job.IsSystemInternal)
              {
                var jobAckedMsg = new BackgroundJobCompletedMsg
                {
                  BackgroundJob = objectMapper.Map<BackgroundJobEntity, BackgroundJobEto>(job),
                  JobId = job.Id,
                  CompletedBy = completedBy,
                  IsManually = manually,
                  CompletionStatus = job.Status,
                  TenantId = job.TenantId,
                  TenantName = CurrentTenant.Name
                };
                await _eventBus.PublishAsync(jobAckedMsg);
              }
              else
              {
                var jobAckedMsg = new BackgroundJobCompletedMsg
                {
                  BackgroundJob = objectMapper.Map<BackgroundJobEntity, BackgroundJobEto>(job),
                  JobId = job.Id,
                  CompletedBy = completedBy,
                  IsManually = manually,
                  CompletionStatus = job.Status,
                  TenantId = job.TenantId,
                  TenantName = CurrentTenant.Name
                };
                await _eventBus.PublishAsync(jobAckedMsg);
              }

              await backgroundJobHubContext.JobCompleted(job);
            }

            await backgroundJobsRepository.UpdateAsync(job, true);
            return execution;
          },
          null,
          logger.Log,
          "CompleteExecutionAsync",
          executionId
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

    private bool InternalRetry(BackgroundJobEntity job, BackgroundJobExecutionEntity lastExecution)
    {
      if (job.TimeoutInMinutes <= 0 || job.MaxRetryAttempts <= 0 || job.CurrentRetryAttempt >= job.MaxRetryAttempts)
      {
        return false;
      }

      if (lastExecution == null || !lastExecution.ExecutionEndTimeUtc.HasValue)
      {
        return false;
      }

      if (lastExecution.Status == BackgroundJobExecutionStatus.Errored || (lastExecution.Status == BackgroundJobExecutionStatus.Cancelled && !lastExecution.IsStatusChangedManually))
      {
        job.NextRetryTimeUtc = lastExecution.ExecutionEndTimeUtc.Value.AddMinutes(job.RetryIntervalInMinutes);
        return true;
      }

      return false;
    }

    private static bool IsFinalExecutionStatus(BackgroundJobExecutionStatus status) =>
        status is BackgroundJobExecutionStatus.Completed
            or BackgroundJobExecutionStatus.Errored
            or BackgroundJobExecutionStatus.Cancelled;

    private static bool IsFinalWithSameOutcome(
        BackgroundJobExecutionStatus status,
        bool successfully) =>
        (successfully && status == BackgroundJobExecutionStatus.Completed) ||
        (!successfully && status == BackgroundJobExecutionStatus.Errored);

    private static bool IsJobFinal(BackgroundJobEntity job) =>
        (job.Status == BackgroundJobStatus.Completed ||
         job.Status == BackgroundJobStatus.Errored ||
         job.Status == BackgroundJobStatus.Cancelled) &&
        job.JobFinishedUtc.HasValue;

    public async Task<KeyValuePair<long, List<BackgroundJobEntity>>> GetBackgroundJobsList(
       string sorting = null,
       int maxResultCount = int.MaxValue,
       int skipCount = 0,
       string searchQuery = null,
       DateTime? creationDateFilterStart = null,
       DateTime? creationDateFilterEnd = null,
       DateTime? lastExecutionDateFilterStart = null,
       DateTime? lastExecutionDateFilterEnd = null,
       IList<string> typeFilter = null,
       IList<BackgroundJobStatus> statusFilter = null)
    {
      KeyValuePair<long, List<BackgroundJobEntity>> result = new();
      try
      {
        result = await backgroundJobsRepository.GetListAsync(
                    sorting, maxResultCount, skipCount,
                    searchQuery,
                    creationDateFilterStart,
                    creationDateFilterEnd,
                    lastExecutionDateFilterStart,
                    lastExecutionDateFilterEnd,
                    typeFilter,
                    statusFilter);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<bool> RetryJob(Guid jobId,
        string startExecutionParams = null,
        string startExecutionExtraParams = null,
        int timeoutInMinutes = -1,
        int maxRetryAttempts = -1,
        int retryInMinutes = -1,
        string onFailureRecepients = null)
    {
      bool result = false;
      try
      {
        using var uow = unitOfWorkManager.Begin(true);
        var job = await backgroundJobsRepository.GetAsync(jobId);

        if (job.Status != BackgroundJobStatus.Errored && job.Status != BackgroundJobStatus.Cancelled)
        {
          throw new Exception("Retry possible only for errored and cancelled jobs");
        }

        if (!job.IsRetryAllowed)
        {
          throw new UserFriendlyException("Retry not allowed for this job");
        }

        if (startExecutionParams != null)
        {
          job.StartExecutionParams = startExecutionParams;
        }

        if (startExecutionExtraParams != null)
        {
          job.StartExecutionExtraParams = startExecutionExtraParams;
        }

        if (timeoutInMinutes >= 0)
        {
          job.TimeoutInMinutes = timeoutInMinutes;
        }

        if (maxRetryAttempts >= 0)
        {
          job.MaxRetryAttempts = maxRetryAttempts;
        }

        if (retryInMinutes >= 0)
        {
          job.RetryIntervalInMinutes = retryInMinutes;
        }

        if (onFailureRecepients != null)
        {
          job.OnFailureRecepients = onFailureRecepients;
        }

        await backgroundJobsRepository.UpdateAsync(job, true);
        await uow.CompleteAsync();

        await StartExecutionAsync(jobId, true);

        return true;
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
  }
}
