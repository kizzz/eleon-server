using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.GatewayManagement.Module.Proxies
{
  public class GatewayWorkspaceDto
  {
    public EventBusProvider EventBusProvider { get; set; }
    public string EventBusProviderOptionsJson { get; set; }
    public bool SelfHostEventBus { get; set; }
    public string VpnAddress { get; set; }
    public string VpnPrivateKey { get; set; }
    public string VpnPublicKey { get; set; }
    public string VpnDns { get; set; }
    public int VpnListenPort { get; set; }
    public string VpnAdapterName { get; set; }
    public Guid VpnAdapterGuid { get; set; }
    public string VpnServerAddress { get; set; }
    public string VpnServerPublicKey { get; set; }
  }
}
