using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.GatewayManagement.Module.Proxies
{
  public class SetGatewayHealthStatusRequestDto
  {
    //string workspaceName, ServiceHealthStatus busHealthStatus
    public string WorkspaceName { get; set; }
    public ServiceHealthStatus HealthStatus { get; set; }
  }
}
