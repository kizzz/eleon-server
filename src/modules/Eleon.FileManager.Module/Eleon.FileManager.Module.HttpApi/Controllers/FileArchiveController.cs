using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.FileManager.Module.FileArchives;

namespace VPortal.FileManager.Module.Controllers
{
  [Area(ModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/file-manager/file-archives")]
  public class FileArchiveController : ModuleController, IFileArchiveAppService
  {
    private readonly IVportalLogger<FileArchiveController> _logger;
    private readonly IFileArchiveAppService _appService;

    public FileArchiveController(
        IVportalLogger<FileArchiveController> logger,
        IFileArchiveAppService appService)
    {
      _logger = logger;
      _appService = appService;
    }

    [HttpGet("GetFileArchiveById")]
    public async Task<FileArchiveDto> GetFileArchiveById(Guid id)
    {

      var response = await _appService.GetFileArchiveById(id);


      return response;
    }

    [HttpPost("CreateFileArchive")]
    public async Task<FileArchiveDto> CreateFileArchive(FileArchiveDto fileArchive)
    {

      var response = await _appService.CreateFileArchive(fileArchive);


      return response;
    }

    [HttpPost("UpdateFileArchive")]
    public async Task<FileArchiveDto> UpdateFileArchive(FileArchiveDto fileArchive)
    {

      var response = await _appService.UpdateFileArchive(fileArchive);


      return response;
    }

    [HttpDelete("DeleteFileArchive")]
    public async Task<bool> DeleteFileArchive(Guid id)
    {

      var response = await _appService.DeleteFileArchive(id);


      return response;
    }

    [HttpGet("GetFileArchivesList")]
    public async Task<List<FileArchiveDto>> GetFileArchivesList()
    {

      var response = await _appService.GetFileArchivesList();


      return response;
    }

    [HttpPost("GetFileArchivesListByParams")]
    public async Task<PagedResultDto<FileArchiveDto>> GetFileArchivesListByParams(FileArchiveListRequestDto input)
    {

      var response = await _appService.GetFileArchivesListByParams(input);


      return response;
    }
  }
}
