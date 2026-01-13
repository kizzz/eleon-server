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
public class GetUsersInRoleMsg : VportalEvent
{
  public string RoleName { get; set; } = string.Empty;
  public string UserNameFilter { get; set; } = string.Empty;
  public int Skip { get; set; } = 0;
  public int Take { get; set; } = int.MaxValue;
  public bool ExclusionMode { get; set; } = false;
}

[DistributedEvent]
public class GetUsersInRoleResponseMsg : VportalEvent
{
  public int TotalCount { get; set; }
  public List<RoleUserLookup> Users { get; set; } = new();
}
