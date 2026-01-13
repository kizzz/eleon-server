using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Module.ETO
{
  public class VpnServerSettingsEto
  {
    public string PrivateKey { get; set; }
    public string PublicKey { get; set; }
    public int ListenPort { get; set; }
    public string Address { get; set; }
    public string Dns { get; set; }

    public List<VpnServerPeerEto> Peers { get; set; }
  }

  public class VpnServerPeerEto
  {
    public string PrivateKey { get; set; }
    public string PublicKey { get; set; }
    public string AllowedIps { get; set; }
    public string Endpoint { get; set; }
    public int PersistentKeepalive { get; set; }
    public string RefId { get; set; }
  }
}
