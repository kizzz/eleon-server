using Common.Module.Events;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.Messages.Permissions;

[DistributedEvent]
public class CheckPermissionRequestMsg : VportalEvent
{
  public string ProviderName { get; set; } = "U"; // string ProviderName
  public string ProviderKey { get; set; } = string.Empty; // ProviderKey
  public List<string> Permissions { get; set; } = new List<string>();
}
