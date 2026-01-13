using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using Logging.Module;
using Messaging.Module.ETO;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Permissions;

namespace VPortal.FileManager.Module.FileExternalLink
{
  public class FileExternalLinkAppService : ModuleAppService, IFileExternalLinkAppService
  {
    private readonly IVportalLogger<FileExternalLinkAppService> logger;
    private readonly Managers.FileManager fileManager;
    private readonly FileExternalLinkDomainService domainService;

    public FileExternalLinkAppService(IVportalLogger<FileExternalLinkAppService> logger,
        Managers.FileManager fileManager,
        FileExternalLinkDomainService domainService)
    {
      this.logger = logger;
      this.fileManager = fileManager;
      this.domainService = domainService;
    }

    public async Task<bool> CancelChanges(Guid linkId)
    {

      bool result = false;
      try
      {
        result = await domainService.Cancel(linkId);
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

    public async Task<bool> CancelChangesByFile(Guid archiveId, string fileId)
    {
      bool result = false;
      try
      {
        var entity = await GetFileExternalLinkSetting(fileId, archiveId);
        if (entity.Id == Guid.Empty)
        {
          return result;
        }
        result = await domainService.Cancel(entity.Id);
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

    public async Task<FileExternalLinkReviewerDto> CreateOrUpdateReviewer(CreateOrUpdateReviewerDto createOrUpdateReviewerDto)
    {

      FileExternalLinkReviewerDto result = null;
      try
      {
        var reviewerEntity = ObjectMapper.Map<FileExternalLinkReviewerDto, FileExternalLinkReviewerEntity>(createOrUpdateReviewerDto.UpdatedReviewer);

        ExternalLinkEto externalLinkEntity = null;
        if (createOrUpdateReviewerDto.ExternalLink != null)
        {
          externalLinkEntity = createOrUpdateReviewerDto.ExternalLink;
        }

        var updatedEntity = await domainService.CreateOrUpdateReviewer(createOrUpdateReviewerDto.FileExternalLinkId, reviewerEntity, externalLinkEntity);

        result = ObjectMapper.Map<FileExternalLinkReviewerEntity, FileExternalLinkReviewerDto>(updatedEntity);
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

    public Task<bool> DeleteExternalLinkSetting(Guid id)
    {
      throw new NotImplementedException();
    }

    public async Task<bool> DeleteReviewer(Guid reviewerId)
    {

      bool result = false;
      try
      {
        result = await domainService.DeleteReviewerAsync(reviewerId);
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

    [Authorize(FileManagerPermissions.Share)]
    public async Task<FileExternalLinkDto> GetFileExternalLinkSetting(string fileId, Guid archiveId)
    {

      FileExternalLinkDto result = null;
      try
      {
        var entity = await domainService.GetAsync(fileId, archiveId);
        result = ObjectMapper.Map<FileExternalLinkEntity, FileExternalLinkDto>(entity);
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

    public async Task<FileExternalLinkReviewerInfoDto> GetLoginInfoAsync(Guid id)
    {

      FileExternalLinkReviewerInfoDto result = null;
      try
      {
        var entity = await domainService.GetReviewerAsync(id);
        var link = await domainService.GetReviewerLink(id);
        var file = await fileManager.GetEntryById(link.FileId, link.ArchiveId, FileManagerType.FileArchive);

        result = new FileExternalLinkReviewerInfoDto()
        {
          ReviewerStatus = entity.ReviewerStatus,
          FileName = file.Name,
          ReviewerType = entity.ReviewerType,
        };
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

    public async Task<string> DirectLoginAsync(Guid id, string password)
    {

      string result = null;
      try
      {
        result = await domainService.DirectLoginAsync(id, password);
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

    public async Task<bool> SaveChanges(Guid linkId, bool deleteAfterChanges)
    {

      bool result = false;
      try
      {
        result = await domainService.SaveExternalChanges(linkId, deleteAfterChanges);
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

    public async Task<bool> SaveChangesByFile(Guid archiveId, string fileId, bool deleteAfterChanges)
    {
      bool result = false;
      try
      {
        var entity = await GetFileExternalLinkSetting(fileId, archiveId);
        if (entity.Id == Guid.Empty)
        {
          return result;
        }
        result = await domainService.SaveExternalChanges(entity.Id, deleteAfterChanges);
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

    [Authorize(FileManagerPermissions.Share)]
    public async Task<FileExternalLinkDto> UpdateExternalLinkSetting(FileExternalLinkDto updatedDto)
    {
      FileExternalLinkDto result = null;
      try
      {
        var updatedEntity = ObjectMapper.Map<FileExternalLinkDto, FileExternalLinkEntity>(updatedDto);
        FileExternalLinkEntity resultEntity = await domainService.CreateAsync(updatedEntity);
        result = ObjectMapper.Map<FileExternalLinkEntity, FileExternalLinkDto>(resultEntity);
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
