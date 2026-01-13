using EventManagementModule.Module.Application.Contracts.Event;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using ModuleCollector.EventManagementModule.EventManagementModule.Module.Application.Contracts.Event;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.EventManagementModule.Module;

namespace VPortal.EventManagementModule.HttpApi.Controllers;

[Area(EventManagementRemoteServiceConsts.ModuleName)]
[RemoteService(Name = EventManagementRemoteServiceConsts.RemoteServiceName)]
[Route("api/EventManagement/Events")]
public class EventController : EventManagementCotroller, IEventAppService
{
  private readonly IVportalLogger<EventController> _logger;
  private readonly IEventAppService _eventAppService;

  public EventController(IVportalLogger<EventController> logger, IEventAppService eventAppService)
  {
    this._logger = logger;
    this._eventAppService = eventAppService;
  }

  [HttpPost("Publish")]
  public async Task PublishAsync(PublishMessageRequestDto input)
  {

    await _eventAppService.PublishAsync(input);

  }

  [HttpGet("RecieveMany")]
  public async Task<RecieveMessagesResponseDto> ReceiveManyAsync(string queueName, int maxCount = 100)
  {

    var response = await _eventAppService.ReceiveManyAsync(queueName, maxCount);


    return response;
  }

  [HttpPost("PublishError")]
  public async Task PublishErrorAsync(string message)
  {

    await _eventAppService.PublishErrorAsync(message);

  }

  [HttpGet("GetList")]
  public async Task<PagedResultDto<EventDto>> GetListAsync(MessagesPagedAndSortedResultRequestDto input)
  {

    var response = await _eventAppService.GetListAsync(input);


    return response;
  }

  [HttpGet("DownloadMessage")]
  public async Task<FullEventDto> DownloadMessageAsync(Guid messageId)
  {

    var response = await _eventAppService.DownloadMessageAsync(messageId);


    return response;
  }
}
