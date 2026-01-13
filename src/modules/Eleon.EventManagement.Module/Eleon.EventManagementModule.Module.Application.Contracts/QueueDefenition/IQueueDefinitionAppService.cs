using EventManagementModule.Module.Application.Contracts.Queue;
using Volo.Abp.Application.Dtos;

namespace EventManagementModule.Module.Application.Contracts.QueueDefenition;
public interface IQueueDefinitionAppService
{
  Task<QueueDefinitionDto> GetAsync(Guid id);
  Task<List<QueueDefinitionDto>> GetAllAsync();
  Task<PagedResultDto<QueueDefinitionDto>> GetListAsync(PagedAndSortedResultRequestDto input);
  Task<QueueDefinitionDto> CreateAsync(CreateQueueDefinitionRequestDto input);
  Task<QueueDefinitionDto> EnsureCreatedAsync(CreateQueueDefinitionRequestDto input);
  Task<QueueDefinitionDto> UpdateAsync(UpdateQueueDefinitionRequestDto input);
  Task DeleteAsync(Guid id);
}
