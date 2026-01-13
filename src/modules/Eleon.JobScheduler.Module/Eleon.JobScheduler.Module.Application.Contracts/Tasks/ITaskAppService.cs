using EleonsoftModuleCollector.JobScheduler.Module.JobScheduler.Module.Application.Contracts.Tasks;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace VPortal.JobScheduler.Module.Tasks
{
  public interface ITaskAppService : IApplicationService
  {
    Task<TaskDto> GetByIdAsync(Guid id);
    Task<bool> RunTaskManuallyAsync(Guid taskId);
    Task<bool> StopTaskAsync(Guid taskId);
    Task<PagedResultDto<TaskHeaderDto>> GetListAsync(TaskListRequestDto request);
    Task<PagedResultDto<TaskExecutionDto>> GetTaskExecutionListAsync(Guid taskId, TaskExecutionListRequestDto requestDto);
    Task<bool> UpdateAsync(TaskHeaderDto task);
    Task<TaskDto> CreateAsync(CreateTaskDto request);
    Task<bool> DeleteAsync(Guid id);
  }
}
