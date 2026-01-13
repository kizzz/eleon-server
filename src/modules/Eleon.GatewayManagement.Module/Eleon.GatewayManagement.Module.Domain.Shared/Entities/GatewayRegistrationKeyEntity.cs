using Common.Module.Constants;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace GatewayManagement.Module.Entities
{
  public class GatewayRegistrationKeyEntity : CreationAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    public virtual Guid? GatewayId { get; set; }
    public virtual string Key { get; set; }
    public virtual DateTime ExpirationDate { get; set; }
    public virtual bool Invalidated { get; set; }
    public virtual bool Multiuse { get; set; }
    public virtual GatewayRegistrationKeyStatus Status { get; set; }

    protected GatewayRegistrationKeyEntity() { }

    public GatewayRegistrationKeyEntity(Guid id)
    {
      Id = id;
    }

    public bool IsExpired() => ExpirationDate <= DateTime.UtcNow;

    public bool IsValid() => !Invalidated && !IsExpired();

    #region NotMapped

    [NotMapped]
    public int ExpiresAfterMs => (int)ExpirationDate.Subtract(DateTime.UtcNow).TotalMilliseconds;

    #endregion
  }
}
