using ModuleCollector.EventManagementModule.EventManagementModule.Module.Application.Contracts.Queue;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EventManagementModule.Module.Application.Contracts.Queue;
public interface IQueueAppService : IApplicationService
{
  Task<QueueDto> GetAsync(QueueRequestDto request);
  Task<List<QueueDto>> GetAllAsync();
  Task<QueueDto> CreateAsync(CreateQueueRequestDto input);
  Task<QueueDto> EnsureCreatedAsync(CreateQueueRequestDto input);
  Task<QueueDto> UpdateAsync(UpdateQueueRequestDto input);
  Task DeleteAsync(QueueRequestDto request);
  Task ClearAsync(QueueRequestDto request);
  Task<PagedResultDto<QueueDto>> GetListAsync(QueuesListRequestDto input);
}
