using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using VPortal.BackgroundJobs.Module.Entities;

namespace BackgroundJobs.Module.BackgroundJobs
{
  public interface IBackgroundJobAppService : IApplicationService
  {
    Task<BackgroundJobDto> CreateAsync(CreateBackgroundJobDto input);
    Task<BackgroundJobDto> CompleteAsync(BackgroundJobExecutionCompleteDto input);
    Task<FullBackgroundJobDto> GetBackgroundJobByIdAsync(Guid id);
    Task<bool> RetryBackgroundJobAsync(Guid id);
    Task<PagedResultDto<BackgroundJobHeaderDto>> GetBackgroundJobListAsync(BackgroundJobListRequestDto input);
    Task<bool> CancelBackgroundJobAsync(Guid id);
    Task<bool> MarkExecutionStartedAsync(Guid jobId, Guid executionId);
  }
}
