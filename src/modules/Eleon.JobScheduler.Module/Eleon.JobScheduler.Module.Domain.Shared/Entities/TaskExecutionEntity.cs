using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.JobScheduler.Module.Entities
{
  [DisableAuditing]
  public class TaskExecutionEntity : FullAuditedEntity<Guid>, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }

    [NotMapped]
    public virtual JobSchedulerTaskExecutionStatus Status {
      get
      {
        if (ActionExecutions == null || ActionExecutions.Count == 0)
        {
          return JobSchedulerTaskExecutionStatus.Initializing;
        }

        var allNotStarted = ActionExecutions.All(x => x.Status == JobSchedulerActionExecutionStatus.NotStarted);
        if (allNotStarted)
          return JobSchedulerTaskExecutionStatus.Initializing;

        var allCompleted = ActionExecutions.All(x => x.Status == JobSchedulerActionExecutionStatus.Completed);
        if (allCompleted)
          return JobSchedulerTaskExecutionStatus.Completed;

        var anyExecuting = ActionExecutions.Any(x => x.Status == JobSchedulerActionExecutionStatus.Executing);
        if (anyExecuting)
          return JobSchedulerTaskExecutionStatus.Executing;

        var anyCancelled = ActionExecutions.Any(x => x.Status == JobSchedulerActionExecutionStatus.Cancelled);
        if (anyCancelled)
          return JobSchedulerTaskExecutionStatus.Cancelled;

        return JobSchedulerTaskExecutionStatus.Failed;
      }
    }
    public virtual Guid? RunnedByUserId { get; set; }
    public virtual string RunnedByUserName { get; set; }
    public virtual Guid? RunnedByTriggerId { get; set; }
    public virtual string RunnedByTriggerName { get; set; }
    public virtual DateTime? StartedAtUtc { get; set; }
    public virtual DateTime? FinishedAtUtc { get; set; }
    public virtual Guid TaskId { get; set; }
    public virtual IList<ActionExecutionEntity> ActionExecutions { get; set; }


    protected TaskExecutionEntity() { }

    public TaskExecutionEntity(Guid id, Guid taskId)
    {
      this.Id = id;
      this.TaskId = taskId;
    }


    #region DisplayProperties
    [NotMapped]
    public virtual bool IsStatusChangedManually => ActionExecutions != null && ActionExecutions.Count > 0 && ActionExecutions.Any(x => x.IsStatusChangedManually);
    #endregion
  }
}
