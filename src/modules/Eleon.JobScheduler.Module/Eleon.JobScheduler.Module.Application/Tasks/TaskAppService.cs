using EleonsoftModuleCollector.JobScheduler.Module.JobScheduler.Module.Application.Contracts.Tasks;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using VPortal.JobScheduler.Module.DomainServices;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.Triggers;

namespace VPortal.JobScheduler.Module.Tasks
{
  public class TaskAppService : JobSchedulerModuleAppService, ITaskAppService
  {
    private readonly IVportalLogger<TaskAppService> logger;
    private readonly TaskDomainService taskService;
    private readonly TaskExecutionManager taskExecutionManager;
    private readonly TaskExecutionDomainService taskExecutionService;

    public TaskAppService(
        IVportalLogger<TaskAppService> logger,
        TaskDomainService taskService,
        TaskExecutionManager taskExecutionManager,
        TaskExecutionDomainService taskExecutionService)
    {
      this.logger = logger;
      this.taskService = taskService;
      this.taskExecutionManager = taskExecutionManager;
      this.taskExecutionService = taskExecutionService;
    }

    public async Task<TaskDto> GetByIdAsync(Guid id)
    {
      TaskDto response = null;
      try
      {
        var gotEntity = await taskService.GetByIdAsync(id);
        response = ObjectMapper.Map<TaskEntity, TaskDto>(gotEntity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<bool> RunTaskManuallyAsync(Guid taskId)
    {
      bool response = false;
      try
      {
        response = await taskExecutionManager.RunTaskManuallyAsync(taskId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<PagedResultDto<TaskHeaderDto>> GetListAsync(TaskListRequestDto request)
    {
      var response = new PagedResultDto<TaskHeaderDto>();
      try
      {
        var gotEntities = await taskService.GetListAsync(
            request.SkipCount,
            request.MaxResultCount,
            request.Sorting,
            request.NameFilter);
        var items = ObjectMapper.Map<List<TaskEntity>, List<TaskHeaderDto>>(gotEntities.Value);
        var total = gotEntities.Key;
        response = new PagedResultDto<TaskHeaderDto>(total, items);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<PagedResultDto<TaskExecutionDto>> GetTaskExecutionListAsync(Guid taskId, TaskExecutionListRequestDto requestDto)
    {
      var response = new PagedResultDto<TaskExecutionDto>();
      try
      {
        var gotEntities = await taskExecutionService.GetTaskExecutionsListAsync(
            taskId,
            requestDto.SkipCount,
            requestDto.MaxResultCount,
            requestDto.Sorting);
        var items = ObjectMapper.Map<List<TaskExecutionEntity>, List<TaskExecutionDto>>(gotEntities.Value);
        var total = gotEntities.Key;
        response = new PagedResultDto<TaskExecutionDto>(total, items);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<bool> UpdateAsync(TaskHeaderDto task)
    {
      bool response = false;
      try
      {
        var entity = ObjectMapper.Map<TaskHeaderDto, TaskEntity>(task);

        response = await taskService.UpdateTask(entity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<TaskDto> CreateAsync(CreateTaskDto request)
    {
      try
      {
        var result = await taskService.CreateAsync(request.Name, request.Description);
        return ObjectMapper.Map<TaskEntity, TaskDto>(result);
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
      try
      {
        var result = await taskService.DeleteAsync(id);
        return true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }
    }

    public Task<bool> StopTaskAsync(Guid taskId)
    {
      try
      {
        return taskExecutionManager.StopTaskAsync(taskId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }
    }
  }
}
