using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.CustomPermissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.TenantManagement.Module.CustomPermissions
{
  public interface ICustomPermissionsAppService : IApplicationService
  {
    Task<bool> CreateBulkForMicroserviceAsync(CustomPermissionsForMicroserviceDto customPermissionsForMicroserviceDto);
    Task<CustomPermissionGroupDto> CreateGroupAsync(CustomPermissionGroupDto createGroupDto);

    Task<CustomPermissionGroupDto> UpdateGroupAsync(CustomPermissionGroupDto updateGroupDto);

    Task DeleteGroupAsync(string name);

    Task<List<CustomPermissionGroupDto>> GetPermissionDynamicGroupCategoriesAsync();

    Task<CustomPermissionDto> CreatePermissionAsync(CustomPermissionDto createPermissionDto);

    Task<CustomPermissionDto> UpdatePermissionAsync(CustomPermissionDto updatePermissionDto);

    Task DeletePermissionAsync(string name);

    Task<List<CustomPermissionDto>> GetPermissionsDynamicAsync();
    Task GrantUserPermissionAsync(Guid userId, string permission, bool isGranted);
  }
}
