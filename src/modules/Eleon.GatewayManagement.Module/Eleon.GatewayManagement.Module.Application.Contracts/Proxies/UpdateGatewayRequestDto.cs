using GatewayManagement.Module.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.GatewayManagement.Module.EventBuses;

namespace VPortal.GatewayManagement.Module.Proxies
{
  public class UpdateGatewayRequestDto
  {
    public GatewayDto Gateway { get; set; }
    public EventBusDto EventBus { get; set; }
    public bool UseDefault { get; set; }
  }
}
