using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.BackgroundJobs.Module.Entities
{
  [DisableAuditing]
  public class BackgroundJobEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public Guid? TenantId { get; set; }

    public Guid? ParentJobId { get; set; }
    public string Type { get; set; }

    /// <summary>
    /// If job created with app service, it is not an internal system job.
    /// </summary>
    public bool IsSystemInternal { get; set; }
    public string SourceId { get; set; } // ApiName or UserId
    public string SourceType { get; set; } // Api or User or SystemModule

    public BackgroundJobStatus Status { get; set; }

    public DateTime ScheduleExecutionDateUtc { get; set; }
    public DateTime LastExecutionDateUtc { get; set; }
    public virtual List<BackgroundJobExecutionEntity> Executions { get; set; }

    /// <summary>
    /// Is manually retry allowed. For system auto retry using MaxRetryAttempts and RetryIntervalInMinutes.
    /// </summary>
    public bool IsRetryAllowed { get; set; }
    public string Initiator { get; set; } // custom name
    public string Description { get; set; }

    public string StartExecutionParams { get; set; }
    public string StartExecutionExtraParams { get; set; }

    public string Result { get; set; }
    public DateTime? JobFinishedUtc { get; set; }

    [Obsolete("EnvironmentId is deprecated")]
    public string EnvironmentId { get; set; } = string.Empty;

    public int TimeoutInMinutes { get; set; } = 0; // 0 means no delay
    public int RetryIntervalInMinutes { get; set; }
    public int MaxRetryAttempts { get; set; }
    public int CurrentRetryAttempt { get; set; }
    public DateTime? NextRetryTimeUtc { get; set; }
    public string OnFailureRecepients { get; set; } = string.Empty;

    public BackgroundJobEntity()
    {
      Executions = new List<BackgroundJobExecutionEntity>();
      StartExecutionParams = string.Empty;
      StartExecutionExtraParams = string.Empty;
      Result = string.Empty;
      EnvironmentId = string.Empty;
      OnFailureRecepients = string.Empty;
    }

    public BackgroundJobEntity(Guid id)
    {
      this.Id = id;
      Executions = new List<BackgroundJobExecutionEntity>();
      StartExecutionParams = string.Empty;
      StartExecutionExtraParams = string.Empty;
      Result = string.Empty;
      EnvironmentId = string.Empty;
      OnFailureRecepients = string.Empty;
    }

    public override string ToString()
    {
      return $"[Job #{Id}, {Type}]";
    }

    public void UpdateStatus(BackgroundJobStatus status)
    {
      if (Status == BackgroundJobStatus.Completed || Status == BackgroundJobStatus.Cancelled)
      {
        return;
      }

      if (status == BackgroundJobStatus.New)
      {
        return;
      }

      if (status == BackgroundJobStatus.Executing && Status == BackgroundJobStatus.Retring)
      {
        return;
      }

      Status = status;
    }
  }
}
