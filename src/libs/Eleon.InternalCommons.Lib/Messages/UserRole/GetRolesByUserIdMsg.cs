using Common.Module.Events;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.TenantManagement.Module.Roles;

namespace Eleon.InternalCommons.Lib.Messages.UserRole;

[DistributedEvent]
public class GetRolesByUserIdMsg : VportalEvent
{
  public Guid UserId { get; set; }
  public UserRoleLookupProviderFormat ProviderFormat { get; set; }
}

[DistributedEvent]
public class GetRolesByUserIdResponseMsg : VportalEvent
{
  public List<UserRoleLookup> Roles { get; set; } = new List<UserRoleLookup>();
}
