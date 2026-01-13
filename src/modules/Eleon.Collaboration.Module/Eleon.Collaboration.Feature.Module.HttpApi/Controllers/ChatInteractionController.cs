using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.Collaboration.Feature.Module.ChatInteractions;
using VPortal.Collaboration.Feature.Module.ChatMessages;

namespace VPortal.Collaboration.Feature.Module.Controllers
{
  [Area(CollaborationRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = CollaborationRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Collaboration/ChatInteraction")]
  public class ChatInteractionController : ChatController, IChatInteractionAppService
  {
    private readonly IChatInteractionAppService appService;
    private readonly IVportalLogger<ChatInteractionController> _logger;

    public ChatInteractionController(
        IChatInteractionAppService appService,
        IVportalLogger<ChatInteractionController> logger)
    {
      this.appService = appService;
      _logger = logger;
    }

    [HttpPost("AckMessageReceived")]
    public async Task<bool> AckMessageReceived(Guid messageId)
    {

      var response = await appService.AckMessageReceived(messageId);


      return response;
    }

    [HttpGet("GetChatById")]
    public async Task<UserChatInfoDto> GetChatByIdAsync(Guid chatId)
    {

      var response = await appService.GetChatByIdAsync(chatId);


      return response;
    }

    [HttpGet("GetChatMessages")]
    public async Task<List<ChatMessageDto>> GetChatMessages(Guid chatId, int skip, int take)
    {

      var response = await appService.GetChatMessages(chatId, skip, take);


      return response;
    }

    [HttpPost("GetLastChats")]
    public async Task<PagedResultDto<UserChatInfoDto>> GetLastChats(LastChatsRequestDto request)
    {

      var response = await appService.GetLastChats(request);


      return response;
    }

    [HttpPost("OpenChat")]
    public async Task<List<ChatMessageDto>> OpenChat(Guid chatId, int limit)
    {

      var response = await appService.OpenChat(chatId, limit);


      return response;
    }

    [HttpGet("RetreiveDocumentMessageContent")]
    public async Task<string> RetreiveDocumentMessageContent(Guid messageId)
    {

      var response = await appService.RetreiveDocumentMessageContent(messageId);


      return response;
    }

    [HttpPost("SendDocumentMessage")]
    public async Task<ChatMessageDto> SendDocumentMessage(SendDocumentMessageRequestDto request)
    {

      var response = await appService.SendDocumentMessage(request);


      return response;
    }

    [HttpPost("SendTextMessage")]
    public async Task<ChatMessageDto> SendTextMessage(SendTextMessageRequestDto request)
    {

      var response = await appService.SendTextMessage(request);


      return response;
    }
  }
}
