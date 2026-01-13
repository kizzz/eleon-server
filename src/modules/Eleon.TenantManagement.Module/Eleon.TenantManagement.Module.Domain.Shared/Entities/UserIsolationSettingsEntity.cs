using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.TenantManagement.Module.Entities
{
  public class UserIsolationSettingsEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public UserIsolationSettingsEntity(Guid id)
    {
      Id = id;
    }

    protected UserIsolationSettingsEntity() { }

    public Guid? TenantId { get; set; }
    public Guid UserId { get; set; }
    public bool UserIsolationEnabled { get; set; }
    public string UserCertificateHash { get; set; }

    [NotMapped]
    public bool TenantIsolationEnabled { get; set; }
  }
}
