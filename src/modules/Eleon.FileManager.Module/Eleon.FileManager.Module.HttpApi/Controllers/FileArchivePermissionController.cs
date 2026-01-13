using Common.Module.Constants;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.FileManager.Module.FileArchivePermissions;

namespace VPortal.FileManager.Module.Controllers
{
  [Area(ModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/file-manager/file-archives-permissions")]
  public class FileArchivePermissionController : ModuleController, IFileArchivePermissionAppService
  {
    private readonly IVportalLogger<FileArchivePermissionController> _logger;
    private readonly IFileArchivePermissionAppService _appService;

    public FileArchivePermissionController(
        IVportalLogger<FileArchivePermissionController> logger,
        IFileArchivePermissionAppService appService)
    {
      _logger = logger;
      _appService = appService;
    }

    [HttpGet("GetList")]
    public async Task<List<FileArchivePermissionDto>> GetList(Guid archiveId, string folderId)
    {

      var response = await _appService.GetList(archiveId, folderId);


      return response;
    }

    [HttpGet("GetPermissionOrDefault")]
    public async Task<IEnumerable<FileManagerPermissionType>> GetPermissionOrDefault(FileArchivePermissionKeyDto key)
    {

      var response = await _appService.GetPermissionOrDefault(key);


      return response;
    }

    [HttpGet("GetPermissionWithoutDefault")]
    public async Task<FileArchivePermissionDto> GetPermissionWithoutDefault(FileArchivePermissionKeyDto key)
    {

      var response = await _appService.GetPermissionWithoutDefault(key);


      return response;
    }

    [HttpPut("UpdatePermission")]
    public async Task<FileArchivePermissionDto> UpdatePermission(FileArchivePermissionDto updatedPermissionDto)
    {

      var response = await _appService.UpdatePermission(updatedPermissionDto);


      return response;
    }

    [HttpPost("DeletePermissions")]
    public async Task<bool> DeletePermissions(FileArchivePermissionDto deletedPermissionDto)
    {

      var response = await _appService.DeletePermissions(deletedPermissionDto);


      return response;
    }
  }
}
