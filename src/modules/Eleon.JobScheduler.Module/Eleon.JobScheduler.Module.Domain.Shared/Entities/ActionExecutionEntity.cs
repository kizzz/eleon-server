using Common.Module.Constants;
using System;
using System.Collections.Generic;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.JobScheduler.Module.Entities
{
  [DisableAuditing]
  public class ActionExecutionEntity : FullAuditedEntity<Guid>, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }

    public virtual Guid TaskExecutionId { get; set; }
    public virtual TaskExecutionEntity TaskExecution { get; set; }
    public virtual string ActionName { get; set; }

    public virtual string EventName { get; set; }

    public virtual JobSchedulerActionExecutionStatus Status { get; set; }

    public virtual DateTime? StartedAtUtc { get; set; }

    public virtual DateTime? CompletedAtUtc { get; set; }

    /// <summary>
    /// Gets or sets the ID of the Job corresponding to this ActionExecution.
    /// </summary>
    public virtual Guid? JobId { get; set; }

    /// <summary>
    /// Gets or sets the list of the parenting ActionExecutions (if any),
    /// i.e. those, whose propotype Action was source of this AcionExecution's prototype Action,
    /// and which are also part of the same Task Execution as this ActionExecution.
    /// </summary>
    public virtual List<ActionExecutionParentEntity> ParentActionExecutions { get; set; } // may be change order with parents

    /// <summary>
    /// Gets or sets the serialized information needed to create a job for the action.
    /// Should be based on the shared type so that the receiving module could deserialize this data.
    /// </summary>
    public virtual string ActionParams { get; set; }

    public virtual string ActionExtraParams { get; set; } = string.Empty;

    public virtual Guid? ActionId { get; set; }

    public virtual ActionEntity Action { get; set; }

    public virtual string StatusChangedBy { get; set; } = string.Empty;
    public virtual bool IsStatusChangedManually { get; set; }

    protected ActionExecutionEntity()
    {
      ActionExtraParams = string.Empty;
      StatusChangedBy = string.Empty;
    }

    public ActionExecutionEntity(Guid id, Guid taskExecutionId)
    {
      this.Id = id;
      this.TaskExecutionId = taskExecutionId;
      ActionExtraParams = string.Empty;
      StatusChangedBy = string.Empty;
    }
  }
}
