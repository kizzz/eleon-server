using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VPortal.Collaboration.Feature.Module.Chats;
using VPortal.Collaboration.Feature.Module.DomainServices;

namespace VPortal.Collaboration.Feature.Module.ChatMembers
{
  [Authorize]
  public class ChatMemberAppService : CollaborationAppService, IChatMemberAppService
  {
    private readonly IVportalLogger<ChatMemberAppService> logger;
    private readonly ChatMemberDomainService chatMemberDomainService;

    public ChatMemberAppService(
        IVportalLogger<ChatMemberAppService> logger,
        ChatMemberDomainService chatMemberDomainService)
    {
      this.logger = logger;
      this.chatMemberDomainService = chatMemberDomainService;
    }

    public async Task<bool> AddChatMembers(AddChatMembersRequestDto request)
    {
      bool result = false;
      try
      {
        await chatMemberDomainService.AddMembers(request.ChatId, request.UserIds);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> KickChatMembers(KickChatMembersRequestDto request)
    {
      bool result = false;
      try
      {
        await chatMemberDomainService.KickMembers(request.ChatId, request.UserIds);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> LeaveChat(Guid chatId, bool closeGroup)
    {
      bool result = false;
      try
      {
        await chatMemberDomainService.LeaveChat(chatId, closeGroup);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> SetMemberRole(Guid chatId, Guid userId, ChatMemberRole memberRole)
    {
      bool result = false;
      try
      {
        await chatMemberDomainService.SetMemberRole(chatId, userId, memberRole);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<ChatMemberInfo>> GetChatMembers(Guid chatId)
    {
      List<ChatMemberInfo> result = null;
      try
      {
        result = await chatMemberDomainService.GetChatMembers(chatId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> CheckMembership(Guid chatId)
    {
      bool result = false;
      try
      {
        result = await chatMemberDomainService.CheckMembership(chatId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> JoinChat(Guid chatId)
    {
      bool result = false;
      try
      {
        await chatMemberDomainService.JoinChatAsync(chatId, CurrentUser.Id.Value);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> JoinChatByUserAsync(Guid chatId, Guid userId)
    {
      bool result = false;
      try
      {
        await chatMemberDomainService.JoinChatAsync(chatId, userId);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
