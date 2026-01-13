using Common.Module.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.Identity.Module.EventServices.Sessions;

[DistributedEvent]
public class RevokeSessionByIdRequestMsg
{
  public string SessionId { get; set; }
}
