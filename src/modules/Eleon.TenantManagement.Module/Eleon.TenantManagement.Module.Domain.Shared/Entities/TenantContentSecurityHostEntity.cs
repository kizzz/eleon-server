using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace VPortal.TenantManagement.Module.Entities
{
  public class TenantContentSecurityHostEntity : FullAuditedEntity<Guid>
  {
    public string Hostname { get; set; }

    public TenantContentSecurityHostEntity(Guid id, string hostname)
    {
      Id = id;
      Hostname = hostname;
    }

    protected TenantContentSecurityHostEntity() { }
  }
}
