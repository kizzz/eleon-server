using Common.Module.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.Messages.Permissions;

[DistributedEvent]
public class SendPermissionsMsg
{
  public List<PermissionGroupDefinitionEto> Groups { get; set; }
  public List<PermissionDefinitionEto> Permissions { get; set; }
}
