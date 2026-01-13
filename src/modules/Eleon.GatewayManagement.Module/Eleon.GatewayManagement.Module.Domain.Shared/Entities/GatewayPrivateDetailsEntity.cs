using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace GatewayManagement.Module.Entities
{
  public class GatewayPrivateDetailsEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    public virtual Guid GatewayId { get; set; }
    public virtual string ClientKey { get; set; }
    public virtual string MachineKey { get; set; }
    public virtual string CertificatePemBase64 { get; set; }

    protected GatewayPrivateDetailsEntity() { }

    public GatewayPrivateDetailsEntity(Guid id)
    {
      Id = id;
    }
  }
}
