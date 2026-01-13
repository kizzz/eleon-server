using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.GatewayManagement.Module.EventBuses
{
  public class EventBusOptionsTemplateDto
  {
    public EventBusProvider Provider { get; set; }
    public string Template { get; set; }
  }
}
