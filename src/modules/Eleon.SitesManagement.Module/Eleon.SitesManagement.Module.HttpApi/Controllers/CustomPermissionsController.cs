using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.CustomPermissions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.SitesManagement.Module.CustomPermissions;

namespace VPortal.SitesManagement.Module.Controllers
{
  [Area(SitesManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = SitesManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/SitesManagement/CustomPermissions")]
  public class CustomPermissionsController : SitesManagementController, ICustomPermissionsAppService
  {
    private readonly IVportalLogger<CustomPermissionsController> logger;
    private readonly ICustomPermissionsAppService customPermissionsAppService;

    public CustomPermissionsController(
        IVportalLogger<CustomPermissionsController> logger,
        ICustomPermissionsAppService customPermissionsAppService)
    {
      this.logger = logger;
      this.customPermissionsAppService = customPermissionsAppService;
    }

    [HttpPost("CreateGroup")]
    public async Task<CustomPermissionGroupDto> CreateGroupAsync(CustomPermissionGroupDto createGroupDto)
    {

      var response = await customPermissionsAppService.CreateGroupAsync(createGroupDto);

      return response;
    }

    [HttpPut("UpdateGroup")]
    public async Task<CustomPermissionGroupDto> UpdateGroupAsync(CustomPermissionGroupDto updateGroupDto)
    {

      var response = await customPermissionsAppService.UpdateGroupAsync(updateGroupDto);

      return response;
    }

    [HttpDelete("DeleteGroup")]
    public async Task DeleteGroupAsync(string name)
    {

      await customPermissionsAppService.DeleteGroupAsync(name);

    }

    [HttpGet("GetPermissionGroups")]
    public async Task<List<CustomPermissionGroupDto>> GetPermissionDynamicGroupCategoriesAsync()
    {

      var response = await customPermissionsAppService.GetPermissionDynamicGroupCategoriesAsync();

      return response;
    }

    [HttpPost("CreatePermission")]
    public async Task<CustomPermissionDto> CreatePermissionAsync(CustomPermissionDto createPermissionDto)
    {

      var response = await customPermissionsAppService.CreatePermissionAsync(createPermissionDto);

      return response;
    }

    [HttpPut("UpdatePermission")]
    public async Task<CustomPermissionDto> UpdatePermissionAsync(CustomPermissionDto updatePermissionDto)
    {

      var response = await customPermissionsAppService.UpdatePermissionAsync(updatePermissionDto);

      return response;
    }

    [HttpDelete("DeletePermission")]
    public async Task DeletePermissionAsync(string name)
    {

      await customPermissionsAppService.DeletePermissionAsync(name);

    }

    [HttpGet("GetPermissions")]
    public async Task<List<CustomPermissionDto>> GetPermissionsDynamicAsync()
    {

      var response = await customPermissionsAppService.GetPermissionsDynamicAsync();

      return response;
    }

    [HttpPost("CreateBulkForMicroserviceAsync")]
    public async Task<bool> CreateBulkForMicroserviceAsync(CustomPermissionsForMicroserviceDto customPermissionsForMicroserviceDto)
    {

      var response = await customPermissionsAppService.CreateBulkForMicroserviceAsync(customPermissionsForMicroserviceDto);

      return response;
    }
  }
}


