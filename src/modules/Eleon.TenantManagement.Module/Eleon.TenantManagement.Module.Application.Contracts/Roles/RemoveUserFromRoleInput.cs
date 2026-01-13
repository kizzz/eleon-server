using System;

namespace VPortal.TenantManagement.Module.Roles
{
  public class RemoveUserFromRoleInput
  {
    public Guid UserId { get; set; }
    public string Role { get; set; }
  }
}
