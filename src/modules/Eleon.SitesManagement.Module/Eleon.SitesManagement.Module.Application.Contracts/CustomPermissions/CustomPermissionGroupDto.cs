using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.SitesManagement.Module.CustomPermissions
{
  public class CustomPermissionGroupDto
  {
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string CategoryName { get; set; }
    public bool Dynamic { get; set; }
    public int Order { get; set; }
    public string SourceId { get; set; }
  }
}


