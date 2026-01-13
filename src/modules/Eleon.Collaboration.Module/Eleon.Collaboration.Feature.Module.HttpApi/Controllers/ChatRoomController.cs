using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Application.Contracts.ChatRooms;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.Collaboration.Feature.Module.ChatRooms;

namespace VPortal.Collaboration.Feature.Module.Controllers
{
  [Area(CollaborationRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = CollaborationRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Collaboration/ChatRooms")]
  public class ChatRoomController : ChatController, IChatRoomAppService
  {
    private readonly IChatRoomAppService appService;
    private readonly IVportalLogger<ChatRoomController> _logger;

    public ChatRoomController(
        IChatRoomAppService appService,
        IVportalLogger<ChatRoomController> logger)
    {
      this.appService = appService;
      _logger = logger;
    }



    [HttpPost("CreateGroupChat")]
    public async Task<ChatRoomDto> CreateChatAsync(CreateChatRequestDto request)
    {

      var response = await appService.CreateChatAsync(request);


      return response;
    }

    [HttpGet("GetChatsList")]
    public async Task<PagedResultDto<ChatRoomDto>> GetChatsList(ChatListRequestDto request)
    {

      var response = await appService.GetChatsList(request);


      return response;
    }

    [HttpPost("RenameChat")]
    public async Task<bool> RenameChat(Guid chatId, string newName)
    {

      var response = await appService.RenameChat(chatId, newName);


      return response;
    }

    [HttpPost("Update")]
    public async Task<ChatRoomDto> UpdateAsync(UpdateChatRequestDto request)
    {

      var response = await appService.UpdateAsync(request);


      return response;
    }

    [HttpPost("Close")]

    public async Task CloseAsync(Guid chatId)
    {

      await appService.CloseAsync(chatId);

    }
  }
}
