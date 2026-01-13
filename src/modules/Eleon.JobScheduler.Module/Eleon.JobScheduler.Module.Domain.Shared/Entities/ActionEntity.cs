using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.JobScheduler.Module.Entities
{
  [Audited]
  public class ActionEntity : Entity<Guid>, IMultiTenant, ISoftDelete
  {
    public virtual string DisplayName { get; set; }
    public virtual string EventName { get; set; } // Job name to start

    public virtual string ActionParams { get; set; } // copy to execution params
    public virtual string ActionExtraParams { get; set; } = string.Empty; // copy to execution params

    public virtual bool IsDeleted { get; set; }
    public virtual Guid? TenantId { get; set; }

    public virtual TimeSpan? RetryInterval { get; set; }
    public virtual int MaxRetryAttempts { get; set; }

    public virtual List<ActionParentEntity> ParentActions { get; set; }

    public virtual TaskEntity Task { get; set; }
    public virtual Guid TaskId { get; set; }

    public virtual int TimeoutInMinutes { get; set; }
    public virtual string OnFailureRecepients { get; set; } = string.Empty;
    public virtual TextFormat ParamsFormat { get; set; }

    protected ActionEntity()
    {
      ActionExtraParams = string.Empty;
      OnFailureRecepients = string.Empty;
    }

    public ActionEntity(Guid id)
    {
      this.Id = id;
    }
  }
}
