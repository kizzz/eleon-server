using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace VPortal.Vpn
{
  public class VpnPeerSettingsEntity : FullAuditedAggregateRoot<Guid>
  {
    public VpnPeerSettingsEntity(Guid id)
    {
      Id = id;
    }

    protected VpnPeerSettingsEntity() { }

    public string PublicKey { get; set; }
    public string PrivateKey { get; set; }
    public string AllowedIps { get; set; }
    public string Endpoint { get; set; }
    public int PersistentKeepalive { get; set; }
    public string RefId { get; set; }
  }
}
