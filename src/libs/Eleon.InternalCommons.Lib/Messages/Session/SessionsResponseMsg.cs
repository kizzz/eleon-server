using Common.Module.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.Identity.Module.Entities;

namespace VPortal.Identity.Module.EventServices.Sessions;

[DistributedEvent]
public class SessionsResponseMsg
{
  public List<UserSessionEto> Sessions { get; set; }
}
