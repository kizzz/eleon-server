using System.Collections.Generic;

namespace Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Roles
{
  public class UserRoleLookupDto
  {
    public string RoleName { get; set; }
    public List<string> Providers { get; set; }
    public bool Editable { get; set; }
  }
}
