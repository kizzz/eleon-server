using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace VPortal.FileManager.Module.FileArchives
{
  public interface IFileArchiveAppService : IApplicationService
  {
    Task<FileArchiveDto> GetFileArchiveById(Guid id);
    Task<FileArchiveDto> CreateFileArchive(FileArchiveDto fileArchive);
    Task<FileArchiveDto> UpdateFileArchive(FileArchiveDto fileArchive);
    Task<bool> DeleteFileArchive(Guid id);
    Task<List<FileArchiveDto>> GetFileArchivesList();
    Task<PagedResultDto<FileArchiveDto>> GetFileArchivesListByParams(FileArchiveListRequestDto input);
  }
}
