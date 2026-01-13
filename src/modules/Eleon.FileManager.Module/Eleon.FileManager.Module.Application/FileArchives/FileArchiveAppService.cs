using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Permissions;

namespace VPortal.FileManager.Module.FileArchives
{
  [Authorize(FileManagerPermissions.General)]
  public class FileArchiveAppService : ModuleAppService, IFileArchiveAppService
  {
    private readonly FileArchiveDomainService fileArchiveDomainService;
    private readonly IVportalLogger<FileArchiveAppService> logger;

    public FileArchiveAppService(
        FileArchiveDomainService fileArchiveDomainService,
        IVportalLogger<FileArchiveAppService> logger)
    {
      this.fileArchiveDomainService = fileArchiveDomainService;
      this.logger = logger;
    }

    [Authorize(FileManagerPermissions.Create)]
    public async Task<FileArchiveDto> CreateFileArchive(FileArchiveDto fileArchive)
    {
      FileArchiveDto result = default;
      try
      {
        var entity = await fileArchiveDomainService.CreateFileArchive(
            fileArchive.Name,
            fileArchive.FileArchiveHierarchyType,
            fileArchive.StorageProviderId,
            fileArchive.IsActive,
            fileArchive.IsPersonalizedArchive,
            fileArchive.PhysicalRootFolderId);

        result = ObjectMapper.Map<FileArchiveEntity, FileArchiveDto>(entity);
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

    [Authorize(FileManagerPermissions.Manage)]
    public async Task<bool> DeleteFileArchive(Guid id)
    {
      bool result = false;
      try
      {
        result = await fileArchiveDomainService.DeleteFileArchive(id);
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

    public async Task<FileArchiveDto> GetFileArchiveById(Guid id)
    {
      FileArchiveDto result = default;
      try
      {
        var entity = await fileArchiveDomainService.GetFileArchiveById(id);
        result = ObjectMapper.Map<FileArchiveEntity, FileArchiveDto>(entity);
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

    public async Task<List<FileArchiveDto>> GetFileArchivesList()
    {
      List<FileArchiveDto> result = default;
      try
      {
        var entities = await fileArchiveDomainService.GetFileArchivesList();
        result = ObjectMapper.Map<List<FileArchiveEntity>, List<FileArchiveDto>>(entities);
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

    public async Task<PagedResultDto<FileArchiveDto>> GetFileArchivesListByParams(FileArchiveListRequestDto input)
    {
      PagedResultDto<FileArchiveDto> result = null;
      try
      {
        var pair = await fileArchiveDomainService
            .GetFileArchivesListByParams(
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount,
                input.SearchQuery);
        var dtos = ObjectMapper
            .Map<List<FileArchiveEntity>, List<FileArchiveDto>>(pair.Value);
        result = new PagedResultDto<FileArchiveDto>(pair.Key, dtos);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    [Authorize(FileManagerPermissions.Manage)]
    public async Task<FileArchiveDto> UpdateFileArchive(FileArchiveDto fileArchive)
    {
      FileArchiveDto result = default;
      try
      {
        var inputEntity = ObjectMapper.Map<FileArchiveDto, FileArchiveEntity>(fileArchive);
        var entity = await fileArchiveDomainService.UpdateFileArchive(inputEntity);
        result = ObjectMapper.Map<FileArchiveEntity, FileArchiveDto>(entity);
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
