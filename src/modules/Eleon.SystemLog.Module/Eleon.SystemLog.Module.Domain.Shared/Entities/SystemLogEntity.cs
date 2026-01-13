using Common.Module.Constants;
using Eleon.Logging.Lib.SystemLog.Contracts;
using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.DocMessageLog.Module.Entities;

[DisableAuditing]
public class SystemLogEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
  public virtual Guid? TenantId { get; set; }
  public virtual string Message { get; set; }
  public virtual SystemLogLevel LogLevel { get; set; }
  public virtual string ApplicationName { get; set; }
  public virtual string InitiatorId { get; set; }
  public virtual string InitiatorType { get; set; }
  public virtual bool IsArchived { get; set; }

  public virtual int Count { get; set; }
  public virtual string Hash { get; set; }

  public SystemLogEntity()
      : base()
  {
  }

  public SystemLogEntity(Guid id)
      : base(id)
  {
  }
}
