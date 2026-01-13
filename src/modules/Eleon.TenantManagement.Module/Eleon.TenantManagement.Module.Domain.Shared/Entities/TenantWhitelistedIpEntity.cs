using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace VPortal.TenantManagement.Module.Entities
{
  public class TenantWhitelistedIpEntity : FullAuditedEntity<Guid>
  {
    public TenantWhitelistedIpEntity(Guid id)
    {
      Id = id;
    }

    protected TenantWhitelistedIpEntity() { }

    public Guid? TenantId { get; set; }
    public string IpAddress { get; set; }
    public bool Enabled { get; set; }

  }
}
