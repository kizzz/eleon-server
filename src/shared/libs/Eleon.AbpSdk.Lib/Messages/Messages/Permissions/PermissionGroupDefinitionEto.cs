using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.Messages.Permissions;
public class PermissionGroupDefinitionEto
{
  public string Name { get; set; } = string.Empty;
  public string DisplayName { get; set; } = string.Empty;
  public Dictionary<string, object?> ExtraProperties { get; set; } = new Dictionary<string, object?>();
}
