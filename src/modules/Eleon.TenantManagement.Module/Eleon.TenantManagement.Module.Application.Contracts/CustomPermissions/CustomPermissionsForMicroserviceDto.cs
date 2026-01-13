using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.TenantManagement.Module.CustomPermissions;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.CustomPermissions
{
  public class CustomPermissionsForMicroserviceDto
  {
    public string SourceId { get; set; }
    public List<CustomPermissionGroupDto> Groups { get; set; }
    public List<CustomPermissionDto> Permissions { get; set; }
  }
}
