using Common.Module.Events;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.InternalCommons.Lib.Messages.Identity;

[DistributedEvent]
public class SeedIdentityRequestMsg
{
  public string AdminEmail { get; set; }
  public string AdminPassword { get; set; }
  public string AdminUserName { get; set; }
  public Guid? TenantId { get; set; }
}
