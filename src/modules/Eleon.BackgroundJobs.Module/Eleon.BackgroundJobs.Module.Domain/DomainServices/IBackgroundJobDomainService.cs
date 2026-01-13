using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Module.Constants;
using VPortal.BackgroundJobs.Module.Entities;

namespace VPortal.BackgroundJobs.Module.DomainServices
{
  public interface IBackgroundJobDomainService
  {
    Task<List<BackgroundJobEntity>> GetCurrentJobsAsync();
    Task<List<BackgroundJobEntity>> GetRetryJobsAsync();
    Task<List<BackgroundJobEntity>> GetByType(string type);
    Task<BackgroundJobEntity> GetAsync(Guid id);
    Task<BackgroundJobEntity> CreateAsync(
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
        Dictionary<string, string> extraProperties = null);
    Task<BackgroundJobExecutionEntity> StartExecutionAsync(Guid backgroundJobId, bool isManualRetry = false, bool autoRetry = false);
    Task CancelJobAsync(Guid jobId, string cancelledBy, bool manually, string cancelledMessage);
    Task<BackgroundJobExecutionEntity> MarkExecutionStartedAsync(Guid jobId, Guid executionId);
    Task<BackgroundJobExecutionEntity> CompleteExecutionAsync(
        Guid jobId,
        Guid executionId,
        bool successfully,
        string retryExecutionParams,
        string retryExecutionExtraParams,
        List<BackgroundJobMessageEntity> messages,
        string result,
        string completedBy,
        bool manually);
    Task<KeyValuePair<long, List<BackgroundJobEntity>>> GetBackgroundJobsList(
       string sorting = null,
       int maxResultCount = int.MaxValue,
       int skipCount = 0,
       string searchQuery = null,
       DateTime? creationDateFilterStart = null,
       DateTime? creationDateFilterEnd = null,
       DateTime? lastExecutionDateFilterStart = null,
       DateTime? lastExecutionDateFilterEnd = null,
       IList<string> typeFilter = null,
       IList<BackgroundJobStatus> statusFilter = null);
    Task<bool> RetryJob(Guid jobId,
        string startExecutionParams = null,
        string startExecutionExtraParams = null,
        int timeoutInMinutes = -1,
        int maxRetryAttempts = -1,
        int retryInMinutes = -1,
        string onFailureRecepients = null);
  }
}

