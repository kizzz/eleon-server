using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Module.Constants
{
  public enum EventBusStatus
  {
    Unknown = 0,
    Connected = 1,
    Initialization = 2,
    Error = 3,
    Disconnected = 4,
  }
}
