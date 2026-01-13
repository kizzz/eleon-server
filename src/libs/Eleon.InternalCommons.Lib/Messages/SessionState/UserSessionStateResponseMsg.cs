using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Module.Messages.SessionState;

[Common.Module.Events.DistributedEvent]
public class UserSessionStateResponseMsg
{
  public Guid UserId { get; set; }
  public bool RequirePeriodicPasswordChange { get; set; }
  public bool PermissionErrorEncountered { get; set; }
}
