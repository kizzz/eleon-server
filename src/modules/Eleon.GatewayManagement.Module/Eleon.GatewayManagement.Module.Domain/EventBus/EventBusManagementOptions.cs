using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusManagement.Module
{
  public class EventBusManagementOptions
  {
    public int BusCheckInterval { get; set; } = 5;
    public int BusConnectionInterval { get; set; } = 5;
  }
}
