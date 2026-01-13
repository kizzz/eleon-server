using Common.Module.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Module.Messages.Session;

[DistributedEvent]
public class UserSessionsRevokedMsg
{
  public Guid UserId { get; set; }
  public string SessionId { get; set; } // Nullable if all sessions are revoked
}
