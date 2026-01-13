using EventManagementModule.Module.Application.Contracts.Queue;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using ModuleCollector.EventManagementModule.EventManagementModule.Module.Application.Contracts.Queue;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.EventManagementModule.Module;

namespace VPortal.EventManagementModule.HttpApi.Controllers;

[Area(EventManagementRemoteServiceConsts.ModuleName)]
[RemoteService(Name = EventManagementRemoteServiceConsts.RemoteServiceName)]
[Route("api/EventManagement/Queues")]
public class QueueController : EventManagementCotroller, IQueueAppService
{
  private readonly IQueueAppService _queueAppService;
  private readonly IVportalLogger<QueueController> _logger;

  public QueueController(IQueueAppService queueAppService, IVportalLogger<QueueController> logger)
  {
    this._queueAppService = queueAppService;
    this._logger = logger;
  }

  [HttpPost("Clear")]
  public async Task ClearAsync(QueueRequestDto request)
  {

    await _queueAppService.ClearAsync(request);

  }

  [HttpPost("Create")]
  public async Task<QueueDto> CreateAsync(CreateQueueRequestDto input)
  {

    var response = await _queueAppService.CreateAsync(input);


    return response;
  }

  [HttpDelete("Delete")]
  public async Task DeleteAsync(QueueRequestDto request)
  {

    await _queueAppService.DeleteAsync(request);

  }

  [HttpPost("EnsureCreated")]
  public async Task<QueueDto> EnsureCreatedAsync(CreateQueueRequestDto input)
  {

    var response = await _queueAppService.EnsureCreatedAsync(input);


    return response;
  }

  [HttpGet("GetAll")]
  public async Task<List<QueueDto>> GetAllAsync()
  {

    var response = await _queueAppService.GetAllAsync();


    return response;
  }

  [HttpGet("Get")]
  public async Task<QueueDto> GetAsync(QueueRequestDto request)
  {

    var response = await _queueAppService.GetAsync(request);


    return response;
  }

  [HttpGet("GetList")]
  public async Task<PagedResultDto<QueueDto>> GetListAsync(QueuesListRequestDto input)
  {

    var response = await _queueAppService.GetListAsync(input);


    return response;
  }

  [HttpPost("Update")]
  public async Task<QueueDto> UpdateAsync(UpdateQueueRequestDto input)
  {

    var response = await _queueAppService.UpdateAsync(input);


    return response;
  }
}
