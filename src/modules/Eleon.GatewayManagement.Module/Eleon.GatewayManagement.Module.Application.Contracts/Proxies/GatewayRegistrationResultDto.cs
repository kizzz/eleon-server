using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.GatewayManagement.Module.Proxies
{
  public class GatewayRegistrationResultDto
  {
    public string ClientKey { get; set; }
    public GatewayStatus Status { get; set; }
  }
}
