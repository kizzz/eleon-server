using Common.Module.Constants;
using System;

namespace GatewayManagement.Module.Proxies
{
  public class GatewayDto
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public GatewayProtocol Protocol { get; set; }
    public string MachineHash { get; set; }
    public string IpAddress { get; set; }
    public int? Port { get; set; }
    public GatewayStatus Status { get; set; }
    public ServiceHealthStatus HealthStatus { get; set; }
    public DateTime? LastHealthCheckTime { get; set; }
    public Guid EventBusId { get; set; }
    public bool AllowApplicationOverride { get; set; }
    public bool EnableGatewayAdmin { get; set; }
    public bool SelfHostEventBus { get; set; }

    public string VpnAddress { get; set; }
    public string VpnAdapterName { get; set; }
    public Guid VpnAdapterGuid { get; set; }
    public string VpnPublicKey { get; set; }
    public string VpnDns { get; set; }
    public int VpnListenPort { get; set; }
  }
}
