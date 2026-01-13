using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.GatewayManagement.Module.Proxies
{
  public class AcceptPendingGatewayRequestDto
  {
    public Guid GatewayId { get; set; }
    public string Name { get; set; }
  }
}
