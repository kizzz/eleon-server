using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace VPortal.Vpn
{
  public class VpnServerSettingsEntity : FullAuditedAggregateRoot<Guid>
  {
    public VpnServerSettingsEntity(Guid id, string networkName)
    {
      Id = id;
      NetworkName = networkName;
    }

    protected VpnServerSettingsEntity() { }

    public string NetworkName { get; set; }
    public string PublicKey { get; set; }
    public string PrivateKey { get; set; }
    public int ListenPort { get; set; }
    public string Address { get; set; }
    public string Dns { get; set; }

    public List<VpnPeerSettingsEntity> Peers { get; set; } = new();
  }
}
