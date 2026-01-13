using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Collaboration.Feature.Module.ChatMembers;
using VPortal.Collaboration.Feature.Module.Chats;

namespace VPortal.Collaboration.Feature.Module.Controllers
{
  [Area(CollaborationRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = CollaborationRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Collaboration/ChatMember")]
  public class ChatMemberController : ChatController, IChatMemberAppService
  {
    private readonly IChatMemberAppService appService;
    private readonly IVportalLogger<ChatMemberController> _logger;

    public ChatMemberController(
        IChatMemberAppService appService,
        IVportalLogger<ChatMemberController> logger)
    {
      this.appService = appService;
      _logger = logger;
    }

    [HttpPost("AddChatMembers")]
    public async Task<bool> AddChatMembers(AddChatMembersRequestDto request)
    {

      var response = await appService.AddChatMembers(request);


      return response;
    }

    [HttpGet("CheckMembership")]
    public async Task<bool> CheckMembership(Guid chatId)
    {

      var response = await appService.CheckMembership(chatId);


      return response;
    }

    [HttpGet("GetChatMembers")]
    public async Task<List<ChatMemberInfo>> GetChatMembers(Guid chatId)
    {

      var response = await appService.GetChatMembers(chatId);


      return response;
    }

    [HttpPost("JoinChat")]
    public async Task<bool> JoinChat(Guid chatId)
    {

      var response = await appService.JoinChat(chatId);


      return response;
    }

    [HttpPost("JoinChatByUser")]
    public async Task<bool> JoinChatByUserAsync(Guid chatId, Guid userId)
    {

      var response = await appService.JoinChatByUserAsync(chatId, userId);


      return response;
    }

    [HttpPost("KickChatMembers")]
    public async Task<bool> KickChatMembers(KickChatMembersRequestDto request)
    {

      var response = await appService.KickChatMembers(request);


      return response;
    }

    [HttpPost("LeaveChat")]
    public async Task<bool> LeaveChat(Guid chatId, bool closeGroup)
    {

      var response = await appService.LeaveChat(chatId, closeGroup);


      return response;
    }

    [HttpPost("SetMemberRole")]
    public async Task<bool> SetMemberRole(Guid chatId, Guid userId, ChatMemberRole memberRole)
    {

      var response = await appService.SetMemberRole(chatId, userId, memberRole);


      return response;
    }
  }
}
