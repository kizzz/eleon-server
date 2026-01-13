using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Module.Messages.Permissions;
public class GetGrantedRolePermissionsMsg
{
  public Guid UserId { get; set; }
}

public class GetGrantedRolePermissionsResponseMsg
{
  public List<string> Permissions { get; set; } = new List<string>();
}
