using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.FileManager.Module.FileExternalLink
{
  public interface IFileExternalLinkAppService : IApplicationService
  {
    public Task<FileExternalLinkReviewerInfoDto> GetLoginInfoAsync(Guid id);
    public Task<FileExternalLinkDto> GetFileExternalLinkSetting(string fileId, Guid archiveId);
    public Task<FileExternalLinkDto> UpdateExternalLinkSetting(FileExternalLinkDto updatedDto);
    public Task<bool> DeleteExternalLinkSetting(Guid id);
    public Task<bool> DeleteReviewer(Guid reviewerId);
    public Task<bool> CancelChanges(Guid linkId);
    public Task<bool> CancelChangesByFile(Guid archiveId, string fileId);
    public Task<bool> SaveChanges(Guid linkId, bool deleteAfterChanges);
    public Task<bool> SaveChangesByFile(Guid archiveId, string fileId, bool deleteAfterChanges);
    public Task<FileExternalLinkReviewerDto> CreateOrUpdateReviewer(CreateOrUpdateReviewerDto createOrUpdateReviewerDto);
    public Task<string> DirectLoginAsync(Guid id, string password);
  }
}
