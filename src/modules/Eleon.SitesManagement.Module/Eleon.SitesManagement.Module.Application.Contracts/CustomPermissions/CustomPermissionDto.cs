using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace VPortal.SitesManagement.Module.CustomPermissions
{
  public class CustomPermissionDto
  {
    public Guid Id { get; set; }

    public string GroupName { get; set; }

    public string Name { get; set; }

    public string ParentName { get; set; }

    public string DisplayName { get; set; }

    public bool IsEnabled { get; set; }

    public MultiTenancySides MultiTenancySide { get; set; }

    public string Providers { get; set; }

    public string StateCheckers { get; set; }

    public int Order { get; set; }
    public bool Dynamic { get; set; }
    public string SourceId { get; set; }
  }
}


