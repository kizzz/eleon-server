using System.Collections.Generic;

namespace VPortal.TenantManagement.Module.Roles
{
  public class UserRoleLookupDto
  {
    public string RoleName { get; set; }
    public List<string> Providers { get; set; }
    public bool Editable { get; set; }
  }
}
