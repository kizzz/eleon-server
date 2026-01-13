using EleonsoftModuleCollector.JobScheduler.Module.JobScheduler.Module.Application.Contracts.Tasks;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.JobScheduler.Module.Tasks;

namespace VPortal.JobScheduler.Module.Controllers
{
  [Area(JobSchedulerModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = JobSchedulerModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/JobScheduler/Tasks")]
  public class TaskController : JobSchedulerModuleController, ITaskAppService
  {
    private readonly ITaskAppService appService;
    private readonly IVportalLogger<TaskController> _logger;

    public TaskController(
        ITaskAppService appService,
        IVportalLogger<TaskController> logger)
    {
      this.appService = appService;
      _logger = logger;
    }

    [HttpPost("Create")]
    public async Task<TaskDto> CreateAsync(CreateTaskDto reqeust)
    {

      var response = await appService.CreateAsync(reqeust);


      return response;
    }

    [HttpGet("GetById")]
    public async Task<TaskDto> GetByIdAsync(Guid id)
    {

      var response = await appService.GetByIdAsync(id);


      return response;
    }

    [HttpPost("RunTaskManually")]
    public async Task<bool> RunTaskManuallyAsync(Guid taskId)
    {

      var response = await appService.RunTaskManuallyAsync(taskId);


      return response;
    }

    [HttpGet("GetList")]
    public async Task<PagedResultDto<TaskHeaderDto>> GetListAsync(TaskListRequestDto request)
    {

      var response = await appService.GetListAsync(request);


      return response;
    }

    [HttpPost("Update")]
    public async Task<bool> UpdateAsync(TaskHeaderDto task)
    {

      var response = await appService.UpdateAsync(task);


      return response;
    }

    [HttpGet("GetTaskExecutionList")]
    public async Task<PagedResultDto<TaskExecutionDto>> GetTaskExecutionListAsync(Guid taskId, TaskExecutionListRequestDto request)
    {

      var response = await appService.GetTaskExecutionListAsync(taskId, request);


      return response;
    }

    [HttpDelete("Delete")]
    public async Task<bool> DeleteAsync(Guid id)
    {

      try
      {
        return await appService.DeleteAsync(id);
      }
      finally
      {
      }
    }

    [HttpPost("StopTask")]
    public Task<bool> StopTaskAsync(Guid taskId)
    {
      try
      {
        return appService.StopTaskAsync(taskId);
      }
      finally
      {
      }
    }
  }
}
