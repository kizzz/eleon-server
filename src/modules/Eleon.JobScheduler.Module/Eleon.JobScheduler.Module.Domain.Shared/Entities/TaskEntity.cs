using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.JobScheduler.Module.Entities
{
  [Audited]
  public class TaskEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    public virtual bool IsActive { get; set; }
    public virtual string Name { get; set; }
    public virtual string Description { get; set; }
    public virtual bool CanRunManually { get; set; }
    public virtual TimeSpan? RestartAfterFailInterval { get; set; }
    public virtual int RestartAfterFailMaxAttempts { get; set; }
    public virtual int CurrentRetryAttempt { get; set; } = 0;

    public virtual TimeSpan? Timeout { get; set; }
    public virtual bool AllowForceStop { get; set; }
    public virtual DateTime? LastRunTimeUtc { get; set; }
    public virtual JobSchedulerTaskStatus Status { get; set; }
    public virtual IList<ActionEntity> Actions { get; set; }
    public virtual string OnFailureRecepients { get; set; } = string.Empty;

    protected TaskEntity()
    {
      Actions = new List<ActionEntity>();
      OnFailureRecepients = string.Empty;
    }

    public TaskEntity(Guid id) : this()
    {
      this.Id = id;
    }

    #region DisplayProperties

    [NotMapped]
    public bool IsRetryEnabled => RestartAfterFailInterval.HasValue && RestartAfterFailMaxAttempts > 0 && RestartAfterFailInterval > TimeSpan.Zero;
    [NotMapped]
    public virtual DateTime? NextRunTimeUtc { get; set; }

    #endregion
  }
}
