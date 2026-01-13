using System.Collections.Generic;

namespace VPortal.TenantManagement.Module.PermissionGroups
{
  public record PermissionGroupCategory(string Name, List<PermissionGroup> PermissionGroups);

  public record PermissionGroup(string Name, bool Dynamic, int Order);
}
