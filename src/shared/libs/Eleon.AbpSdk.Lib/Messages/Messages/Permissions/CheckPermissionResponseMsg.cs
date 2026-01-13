using Common.Module.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.Messages.Permissions;

[DistributedEvent]
public class CheckPermissionResponseMsg
{
  public bool IsSuccessful { get; set; }
  public Dictionary<string, bool> PermissionGrants { get; set; } = new Dictionary<string, bool>();
  public string ErrorMessage { get; set; } = string.Empty;
}
