using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace EleonsoftSdk.Messages.Permissions;
public class PermissionDefinitionEto
{
  public string Name { get; set; }
  public string DisplayName { get; set; }
  public string GroupName { get; set; }
  public string ParentName { get; set; }
  public bool IsEnabled { get; set; }
  public MultiTenancySides MultiTenancySide { get; set; }
  public string Providers { get; set; }
  public string StateCheckers { get; set; }
  public Dictionary<string, object?> ExtraProperties { get; set; } = new Dictionary<string, object?>();
}
