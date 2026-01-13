using System.Collections.Generic;
using Volo.Abp.Identity;

namespace Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Roles
{
  public class RoleUserLookupDto
  {
    public IdentityUserDto User { get; set; }
    public List<string> Providers { get; set; }
    public bool Editable { get; set; }
  }
}
