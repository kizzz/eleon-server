using System.Collections.Generic;

namespace VPortal.TenantManagement.Module.Roles
{
  public class UserRoleLookup
  {
    public string RoleName { get; set; }
    public List<string> Providers { get; set; }

    public bool Editable => Providers.Count == 0;
  }

  public enum UserRoleLookupProviderFormat
  {
    Name = 0,
    Id = 1,
  }
}
