using Common.Module.Constants;
using System;
using System.Collections.Generic;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.BackgroundJobs.Module.Entities
{
  [DisableAuditing]
  public class BackgroundJobExecutionEntity : Entity<Guid>, IHasCreationTime, IMultiTenant
  {
    public Guid? TenantId { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime ExecutionStartTimeUtc { get; set; }
    public DateTime? ExecutionEndTimeUtc { get; set; }
    public BackgroundJobExecutionStatus Status { get; set; }
    public bool IsRetryExecution { get; set; }
    public Guid? RetryUserInitiatorId { get; set; }
    public string StartExecutionParams { get; set; }
    public string StartExecutionExtraParams { get; set; }
    public Guid BackgroundJobEntityId { get; set; }
    public virtual List<BackgroundJobMessageEntity> Messages { get; set; }

    public string StatusChangedBy { get; set; } = string.Empty;
    public bool IsStatusChangedManually { get; set; }

    public BackgroundJobExecutionEntity()
    {
      Messages = new List<BackgroundJobMessageEntity>();
      StartExecutionParams = string.Empty;
      StartExecutionExtraParams = string.Empty;
      StatusChangedBy = string.Empty;
    }

    public BackgroundJobExecutionEntity(Guid id)
    {
      this.Id = id;
      Messages = new List<BackgroundJobMessageEntity>();
      StartExecutionParams = string.Empty;
      StartExecutionExtraParams = string.Empty;
      StatusChangedBy = string.Empty;
    }

    public void UpdateStatus(BackgroundJobExecutionStatus status)
    {
      if (Status == BackgroundJobExecutionStatus.Cancelled || Status == BackgroundJobExecutionStatus.Errored)
      {
        return;
      }

      if (Status == BackgroundJobExecutionStatus.Completed && status != BackgroundJobExecutionStatus.Errored)
      {
        return;
      }

      if (status == BackgroundJobExecutionStatus.Starting)
      {
        return;
      }

      if (status == BackgroundJobExecutionStatus.Started)
      {
        ExecutionStartTimeUtc = DateTime.UtcNow;
      }

      if (status == BackgroundJobExecutionStatus.Completed || status == BackgroundJobExecutionStatus.Errored || status == BackgroundJobExecutionStatus.Cancelled)
      {
        ExecutionEndTimeUtc = DateTime.UtcNow;
      }

      Status = status;
    }

    public void AddMessage(BackgroundJobMessageType type, string text, DateTime? dateTime = null)
    {
      dateTime ??= DateTime.UtcNow;
      Messages ??= new List<BackgroundJobMessageEntity>();
      Messages.Add(new BackgroundJobMessageEntity(Guid.NewGuid())
      {
        MessageType = type,
        TextMessage = text,
        CreationTime = dateTime.Value
      });
    }
  }
}
