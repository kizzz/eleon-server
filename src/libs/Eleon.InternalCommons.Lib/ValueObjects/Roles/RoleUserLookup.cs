using System.Collections.Generic;
using Volo.Abp.Identity;

namespace VPortal.TenantManagement.Module.Roles
{
  public class RoleUserLookup
  {
    public IdentityUser User { get; set; }
    public List<string> Providers { get; set; }

    public bool Editable => Providers.Count == 0;
  }
}
