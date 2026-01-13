using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.GatewayManagement.Module.EventBuses
{
  public class EventBusDto
  {
    public Guid Id { get; set; }
    public EventBusProvider Provider { get; set; }
    public string ProviderOptions { get; set; }
    public EventBusStatus Status { get; set; }
    public bool IsDefault { get; set; }
    public string Name { get; set; }
  }
}
