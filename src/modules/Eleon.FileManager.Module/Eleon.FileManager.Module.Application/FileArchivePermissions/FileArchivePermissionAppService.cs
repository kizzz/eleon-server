using Common.Module.Constants;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.ValueObjects;

namespace VPortal.FileManager.Module.FileArchivePermissions
{
  public class FileArchivePermissionAppService : ModuleAppService, IFileArchivePermissionAppService
  {
    private readonly FileArchivePermissionManagementDomainService managementService;
    private readonly IVportalLogger<FileArchivePermissionAppService> logger;
    private readonly FileArchivePermissionCheckerDomainService checkerService;

    public FileArchivePermissionAppService(
        FileArchivePermissionManagementDomainService managementService,
        IVportalLogger<FileArchivePermissionAppService> logger,
        FileArchivePermissionCheckerDomainService checkerService)
    {
      this.managementService = managementService;
      this.logger = logger;
      this.checkerService = checkerService;
    }

    public async Task<List<FileArchivePermissionDto>> GetList(Guid archiveId, string folderId)
    {

      List<FileArchivePermissionDto> result = null;
      try
      {
        var entities = await managementService.GetListAsync(archiveId, new List<string>() { folderId });
        result = ObjectMapper.Map<List<FileArchivePermissionEntity>, List<FileArchivePermissionDto>>(entities);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;

    }

    public async Task<IEnumerable<FileManagerPermissionType>> GetPermissionOrDefault(FileArchivePermissionKeyDto key)
    {
      IEnumerable<FileManagerPermissionType> result = null;
      try
      {
        var keyValueObject = ObjectMapper.Map<FileArchivePermissionKeyDto, FileArchivePermissionKeyValueObject>(key);
        IEnumerable<FileManagerPermissionType> entity = await checkerService.GetAllowedPermissionsForCurrentUser(keyValueObject);
        result = entity;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;
    }

    public async Task<FileArchivePermissionDto> GetPermissionWithoutDefault(FileArchivePermissionKeyDto key)
    {
      FileArchivePermissionDto result = null;
      try
      {
        var keyValueObject = ObjectMapper.Map<FileArchivePermissionKeyDto, FileArchivePermissionKeyValueObject>(key);
        var entity = await managementService.GetPermissionWithoutDefault(keyValueObject);
        result = ObjectMapper.Map<FileArchivePermissionEntity, FileArchivePermissionDto>(entity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;
    }

    public async Task<FileArchivePermissionDto> UpdatePermission(FileArchivePermissionDto updatedPermissionDto)
    {
      FileArchivePermissionDto result = null;
      try
      {
        var updatedPermissionValueObject = ObjectMapper.Map<FileArchivePermissionDto, FileArchivePermissionValueObject>(updatedPermissionDto);
        var entity = await managementService.UpdatePermission(updatedPermissionValueObject);
        result = ObjectMapper.Map<FileArchivePermissionEntity, FileArchivePermissionDto>(entity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;
    }

    public async Task<bool> DeletePermissions(FileArchivePermissionDto deletedPermissionDto)
    {
      bool result = false;
      try
      {
        var deletedPermissionValueObject = ObjectMapper.Map<FileArchivePermissionDto, FileArchivePermissionValueObject>(deletedPermissionDto);
        result = await managementService.DeletePermissions(deletedPermissionValueObject);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }
  }
}
