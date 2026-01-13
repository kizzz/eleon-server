using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.FileManager.Module.FileExternalLink;

namespace VPortal.FileManager.Module.Controllers
{
  [Area(ModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/file-manager/file-external-link")]
  public class FileExternalLinkController : ModuleController, IFileExternalLinkAppService
  {
    private readonly IVportalLogger<FileExternalLinkController> _logger;
    private readonly IFileExternalLinkAppService _appService;

    public FileExternalLinkController(
        IVportalLogger<FileExternalLinkController> logger,
        IFileExternalLinkAppService appService)
    {
      _logger = logger;
      _appService = appService;
    }

    [HttpGet("DeleteExternalLinkSetting")]
    public async Task<bool> DeleteExternalLinkSetting(Guid id)
    {

      var response = await _appService.DeleteExternalLinkSetting(id);


      return response;
    }

    [HttpGet("GetFileExternalLinkSetting")]
    public async Task<FileExternalLinkDto> GetFileExternalLinkSetting(string fileId, Guid archiveId)
    {

      var response = await _appService.GetFileExternalLinkSetting(fileId, archiveId);


      return response;
    }

    [HttpPut("UpdateExternalLinkSetting")]
    public async Task<FileExternalLinkDto> UpdateExternalLinkSetting(FileExternalLinkDto updatedDto)
    {

      var response = await _appService.UpdateExternalLinkSetting(updatedDto);


      return response;
    }

    [HttpGet("GetLoginInfoAsync")]
    public async Task<FileExternalLinkReviewerInfoDto> GetLoginInfoAsync(Guid id)
    {

      var response = await _appService.GetLoginInfoAsync(id);


      return response;
    }

    [HttpDelete("CancelChanges")]
    public async Task<bool> CancelChanges(Guid linkId)
    {

      var response = await _appService.CancelChanges(linkId);


      return response;
    }

    [HttpPost("SaveChanges")]
    public async Task<bool> SaveChanges(Guid linkId, bool deleteAfterChanges)
    {

      var response = await _appService.SaveChanges(linkId, deleteAfterChanges);


      return response;
    }

    [HttpPost("SaveChangesByFile")]
    public async Task<bool> SaveChangesByFile(Guid archiveId, string fileId, bool deleteAfterChanges)
    {

      var response = await _appService.SaveChangesByFile(archiveId, fileId, deleteAfterChanges);


      return response;
    }

    [HttpPost("CancelChangesByFile")]
    public async Task<bool> CancelChangesByFile(Guid archiveId, string fileId)
    {

      var response = await _appService.CancelChangesByFile(archiveId, fileId);


      return response;
    }
    [HttpPut("CreateOrUpdateReviewer")]
    public async Task<FileExternalLinkReviewerDto> CreateOrUpdateReviewer(CreateOrUpdateReviewerDto createOrUpdateReviewerDto)
    {

      var response = await _appService.CreateOrUpdateReviewer(createOrUpdateReviewerDto);


      return response;
    }

    [HttpDelete("DeleteReviewer")]
    public async Task<bool> DeleteReviewer(Guid reviewerId)
    {

      var response = await _appService.DeleteReviewer(reviewerId);


      return response;
    }

    [HttpPost("DirectLoginAsync")]
    public async Task<string> DirectLoginAsync(Guid id, string password)
    {

      var response = await _appService.DirectLoginAsync(id, password);


      return response;
    }

  }
}
