using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.SitesManagement.Module.CustomPermissions;

namespace ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.CustomPermissions
{
  public class CustomPermissionsForMicroserviceDto
  {
    public List<CustomPermissionGroupDto> Groups { get; set; }
    public List<CustomPermissionDto> Permissions { get; set; }
  }
}


