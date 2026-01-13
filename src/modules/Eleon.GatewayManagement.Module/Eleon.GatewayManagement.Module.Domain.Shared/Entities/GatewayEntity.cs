using Common.Module.Constants;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace GatewayManagement.Module.Entities
{
  public class GatewayEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    public virtual string Name { get; set; }
    public virtual GatewayProtocol Protocol { get; set; }
    public virtual string IpAddress { get; set; }
    public virtual string MachineHash { get; set; }
    public virtual int? Port { get; set; }
    public virtual GatewayStatus Status { get; set; }
    public virtual bool AllowApplicationOverride { get; set; }
    public virtual bool EnableGatewayAdmin { get; set; }

    public virtual Guid? EventBusId { get; set; }
    public virtual ServiceHealthStatus HealthStatus { get; set; }
    public virtual DateTime? LastHealthCheckTime { get; set; }
    public virtual bool SelfHostEventBus { get; set; }

    public virtual string VpnAddress { get; set; }
    public virtual string VpnAdapterName { get; set; }
    public virtual Guid VpnAdapterGuid { get; set; }
    public virtual string VpnPrivateKey { get; set; }
    public virtual string VpnPublicKey { get; set; }
    public virtual string VpnDns { get; set; }
    public virtual int VpnListenPort { get; set; }

    protected GatewayEntity() { }

    public GatewayEntity(Guid id, string name, GatewayProtocol protocol)
    {
      Id = id;
      Name = name;
      Protocol = protocol;
    }
  }
}
