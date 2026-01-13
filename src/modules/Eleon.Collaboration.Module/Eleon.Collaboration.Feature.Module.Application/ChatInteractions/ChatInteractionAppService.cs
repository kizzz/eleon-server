using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using VPortal.Collaboration.Feature.Module.ChatMessages;
using VPortal.Collaboration.Feature.Module.Chats;
using VPortal.Collaboration.Feature.Module.DomainServices;
using VPortal.Collaboration.Feature.Module.Entities;

namespace VPortal.Collaboration.Feature.Module.ChatInteractions
{
  [Authorize]
  public class ChatInteractionAppService : CollaborationAppService, IChatInteractionAppService
  {
    private readonly IVportalLogger<ChatInteractionAppService> logger;
    private readonly ChatInteractionDomainService chatInteractionDomainService;

    public ChatInteractionAppService(
        IVportalLogger<ChatInteractionAppService> logger,
        ChatInteractionDomainService chatInteractionDomainService)
    {
      this.logger = logger;
      this.chatInteractionDomainService = chatInteractionDomainService;
    }

    public async Task<bool> AckMessageReceived(Guid messageId)
    {
      bool result = false;
      try
      {
        await chatInteractionDomainService.AckMessageReceived(messageId);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<ChatMessageDto>> GetChatMessages(Guid chatId, int skip, int take)
    {
      List<ChatMessageDto> result = null;
      try
      {
        var entities = await chatInteractionDomainService.GetChatMessages(chatId, skip, take);
        result = ObjectMapper.Map<List<ChatMessageEntity>, List<ChatMessageDto>>(entities);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<PagedResultDto<UserChatInfoDto>> GetLastChats(LastChatsRequestDto request)
    {
      PagedResultDto<UserChatInfoDto> result = null;
      try
      {
        var list = await chatInteractionDomainService.GetLastChats(
            request.Skip,
            request.Take,
            request.NameFilter,
            request.ChatRoomTypes,
            request.Tags,
            isArchived: request.IsArchived,
            isChannels: request.IsChannel);
        var dtos = ObjectMapper.Map<List<UserChatInfo>, List<UserChatInfoDto>>(list.Value);
        result = new PagedResultDto<UserChatInfoDto>(list.Key, dtos);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<UserChatInfoDto> GetChatByIdAsync(Guid chatId)
    {
      try
      {
        var entitiy = await chatInteractionDomainService.GetByIdAsync(chatId);
        var result = ObjectMapper.Map<UserChatInfo, UserChatInfoDto>(entitiy);
        return result;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
      return null;
    }

    public async Task<List<ChatMessageDto>> OpenChat(Guid chatId, int limit)
    {
      List<ChatMessageDto> result = null;
      try
      {
        var entities = await chatInteractionDomainService.OpenChat(chatId, limit);
        result = ObjectMapper.Map<List<ChatMessageEntity>, List<ChatMessageDto>>(entities);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<string> RetreiveDocumentMessageContent(Guid messageId)
    {
      string result = null;
      try
      {
        result = await chatInteractionDomainService.RetreiveDocumentMessageContent(messageId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<ChatMessageDto> SendDocumentMessage(SendDocumentMessageRequestDto request)
    {
      ChatMessageDto result = null;
      try
      {
        var entity = await chatInteractionDomainService.SendDocumentMessage(request.ChatId, request.Filename, request.DocumentBase64);
        result = ObjectMapper.Map<ChatMessageEntity, ChatMessageDto>(entity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<ChatMessageDto> SendTextMessage(SendTextMessageRequestDto request)
    {
      ChatMessageDto result = null;
      try
      {
        var entity = await chatInteractionDomainService.SendTextMessage(request.ChatId, request.Message, request.Severity);
        result = ObjectMapper.Map<ChatMessageEntity, ChatMessageDto>(entity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
