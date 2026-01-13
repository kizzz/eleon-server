using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.Files;
using VPortal.FileManager.Module.Folders;
using VPortal.FileManager.Module.VirtualFolders;

namespace VPortal.FileManager.Module.Controllers
{
  [Area(ModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/file-manager/files")]
  public class FileController : ModuleController, IFileAppService
  {
    private readonly IVportalLogger<FileController> _logger;
    private readonly IFileAppService _appService;

    public FileController(
        IVportalLogger<FileController> logger,
        IFileAppService appService)
    {
      _logger = logger;
      _appService = appService;
    }

    // Unified query endpoints
    [HttpGet("GetEntriesByParentId")]
    public async Task<List<FileSystemEntryDto>> GetEntriesByParentId(string parentId, Guid archiveId, EntryKind? kind, List<FileStatus> fileStatuses, FileManagerType type, bool recursive)
    {
      var response = await _appService.GetEntriesByParentId(parentId, archiveId, kind, fileStatuses, type, recursive);
      return response;
    }

    [HttpGet("GetEntriesByParentIdPaged")]
    public async Task<PagedResultDto<FileSystemEntryDto>> GetEntriesByParentIdPaged(
        [FromQuery] GetFileEntriesByParentPagedInput input,
        [FromQuery] Guid archiveId,
        [FromQuery] EntryKind? kind,
        [FromQuery] List<FileStatus> fileStatuses,
        [FromQuery] FileManagerType type,
        [FromQuery] bool recursive
        )
    {
      var response = await _appService.GetEntriesByParentIdPaged(input, archiveId, kind, fileStatuses, type, recursive);
      return response;
    }

    [HttpPost("GetEntriesByIds")]
    public async Task<List<FileSystemEntryDto>> GetEntriesByIds(List<string> ids, Guid archiveId, EntryKind? kind, FileManagerType type)
    {
      var response = await _appService.GetEntriesByIds(ids, archiveId, kind, type);
      return response;
    }

    [HttpGet("GetEntryById")]
    public async Task<FileSystemEntryDto> GetEntryById(string id, Guid archiveId, FileManagerType type)
    {
      var response = await _appService.GetEntryById(id, archiveId, type);
      return response;
    }

    [HttpGet("GetEntryParentsById")]
    public async Task<List<HierarchyFolderDto>> GetEntryParentsById(string id, Guid archiveId, FileManagerType type)
    {
      var response = await _appService.GetEntryParentsById(id, archiveId, type);
      return response;
    }

    [HttpGet("GetRootEntry")]
    public async Task<FileSystemEntryDto> GetRootEntry(Guid archiveId, FileManagerType type)
    {
      var response = await _appService.GetRootEntry(archiveId, type);
      return response;
    }

    [HttpGet("SearchEntries")]
    public async Task<List<FileSystemEntryDto>> SearchEntries(string search, Guid archiveId, EntryKind? kind, FileManagerType type)
    {
      var response = await _appService.SearchEntries(search, archiveId, kind, type);
      return response;
    }

    // Unified mutation endpoints
    [HttpPost("CreateEntry")]
    public async Task<FileSystemEntryDto> CreateEntry(CreateEntryDto dto, Guid archiveId, FileManagerType type)
    {
      var response = await _appService.CreateEntry(dto, archiveId, type);
      return response;
    }

    [HttpPut("RenameEntry")]
    public async Task<FileSystemEntryDto> RenameEntry(RenameEntryDto dto, Guid archiveId, FileManagerType type)
    {
      var response = await _appService.RenameEntry(dto, archiveId, type);
      return response;
    }

    [HttpPost("MoveEntry")]
    public async Task<bool> MoveEntry(MoveEntryDto dto, Guid archiveId, FileManagerType type)
    {
      var response = await _appService.MoveEntry(dto, archiveId, type);
      return response;
    }

    [HttpPost("MoveAllEntries")]
    public async Task<bool> MoveAllEntries(MoveAllEntriesDto dto, Guid archiveId, FileManagerType type)
    {
      var response = await _appService.MoveAllEntries(dto, archiveId, type);
      return response;
    }

    [HttpPost("CopyEntry")]
    public async Task<bool> CopyEntry(CopyEntryDto dto, Guid archiveId, FileManagerType type)
    {
      var response = await _appService.CopyEntry(dto, archiveId, type);
      return response;
    }

    [HttpDelete("DeleteEntry")]
    public async Task<bool> DeleteEntry(string id, Guid archiveId, FileManagerType type)
    {
      var response = await _appService.DeleteEntry(id, archiveId, type);
      return response;
    }

    // File-content operations (file-only)
    [HttpGet("GetFileToken")]
    public async Task<string> GetFileToken(string id, bool isVersion, Guid archiveId, FileManagerType type)
    {
      var response = await _appService.GetFileToken(id, isVersion, archiveId, type);
      return response;
    }

    [HttpDelete("DeleteFileToken")]
    public async Task<bool> DeleteFileToken(Guid token, Guid archiveId, FileManagerType type)
    {
      var response = await _appService.DeleteFileToken(token, archiveId, type);
      return response;
    }

    [HttpGet("GetFileByToken")]
    public async Task<byte[]> GetFileByToken(string id, Guid token, bool isVersion, Guid archiveId, FileManagerType type)
    {
      var response = await _appService.GetFileByToken(id, token, isVersion, archiveId, type);
      return response;
    }


    [HttpGet("FileViewer")]
    public async Task<FileSourceDto> FileViewer(string id, Guid archiveId, FileManagerType type)
    {
      var response = await _appService.FileViewer(id, archiveId, type);
      return response;
    }

    [HttpGet("DownloadFile")]
    public async Task<byte[]> DownloadFile(string id, bool isVersion, Guid archiveId, FileManagerType type)
    {
      var response = await _appService.DownloadFile(id, isVersion, archiveId, type);
      return response;
    }

    [HttpGet("DownloadFileByToken")]
    public async Task<byte[]> DownloadFileByToken(string id, string token, bool isVersion, Guid archiveId, FileManagerType type)
    {
      var response = await _appService.DownloadFileByToken(id, token, isVersion, archiveId, type);
      return response;
    }

    [HttpGet("ReadTextFile")]
    public async Task<List<string>> ReadTextFile(string id, bool isVersion, Guid archiveId, FileManagerType type)
    {
      var response = await _appService.ReadTextFile(id, isVersion, archiveId, type);
      return response;
    }

    [HttpGet("ReadTextFileByToken")]
    public async Task<List<string>> ReadTextFileByToken(string id, string token, bool isVersion, Guid archiveId, FileManagerType type)
    {
      var response = await _appService.ReadTextFileByToken(id, token, isVersion, archiveId, type);
      return response;
    }


    [HttpPost("UploadFiles")]
    public async Task<List<FileSystemEntryDto>> UploadFiles(FileUploadDto fileUploadDto)
    {
      var response = await _appService.UploadFiles(fileUploadDto);
      return response;
    }

    [HttpPost("DownloadAll")]
    public async Task<byte[]> DownloadAll(DownloadAllDto downloadAllDto)
    {
      var response = await _appService.DownloadAll(downloadAllDto);
      return response;
    }

    [HttpPost("GetEntries")]
    public async Task<List<FileSystemEntryDto>> GetEntries(EntryFilterDto entryFilterDto)
    {
      var response = await _appService.GetEntries(entryFilterDto);
      return response;
    }

    [HttpPost("RestoreEntry")]
    public async Task<bool> RestoreEntry(string id, Guid archiveId, FileManagerType type)
    {
      var response = await _appService.RestoreEntry(id, archiveId, type);
      return response;
    }

    [HttpGet("GetEntryHistory")]
    public async Task<List<FileSystemEntryDto>> GetEntryHistory(string id, Guid archiveId, FileManagerType type)
    {
      var response = await _appService.GetEntryHistory(id, archiveId, type);
      return response;
    }
  }
}
