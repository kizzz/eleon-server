using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Application.Contracts.ChatRooms;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using VPortal.Collaboration.Feature.Module.DomainServices;
using VPortal.Collaboration.Feature.Module.Entities;

namespace VPortal.Collaboration.Feature.Module.ChatRooms
{
  [Authorize]
  public class ChatRoomAppService : CollaborationAppService, IChatRoomAppService
  {
    private readonly IVportalLogger<ChatRoomAppService> logger;
    private readonly ChatRoomDomainService chatRoomDomainService;

    public ChatRoomAppService(
        IVportalLogger<ChatRoomAppService> logger,
        ChatRoomDomainService chatRoomDomainService)
    {
      this.logger = logger;
      this.chatRoomDomainService = chatRoomDomainService;
    }

    public async Task<bool> RenameChat(Guid chatId, string newName)
    {
      bool result = false;
      try
      {
        await chatRoomDomainService.RenameChat(chatId, newName);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<ChatRoomDto> CreateChatAsync(CreateChatRequestDto request)
    {
      ChatRoomDto result = null;
      try
      {
        var entity = await chatRoomDomainService.CreateChatAsync(
            name: request.ChatName,
            refId: null,
            type: ChatRoomType.Group,
            initialMembers: request.InitialMembers,
            setOwner: request.SetOwner && CurrentUser.Id.HasValue,
            isPublic: request.IsPublic,
            tags: request.Tags,
            allowedRoles: request.AllowedRoles,
            allowedOrgUnits: request.AllowedOrgUnits,
            defaultRole: request.DefaultRole
            );
        result = ObjectMapper.Map<ChatRoomEntity, ChatRoomDto>(entity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<ChatRoomDto> UpdateAsync(UpdateChatRequestDto request)
    {

      try
      {
        var entity = await chatRoomDomainService.UpdateChatAsync(
            chatId: request.ChatId,
            name: request.ChatName,
            tags: request.Tags,
            isPublic: request.IsPublic,
            defaultRole: request.DefaultRole
            );
        return ObjectMapper.Map<ChatRoomEntity, ChatRoomDto>(entity);
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

    public async Task CloseAsync(Guid chatId)
    {
      try
      {
        await chatRoomDomainService.CloseAsync(chatId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
    }

    public async Task<PagedResultDto<ChatRoomDto>> GetChatsList(ChatListRequestDto request)
    {
      PagedResultDto<ChatRoomDto> result = null;
      try
      {
        var list = await chatRoomDomainService.GetChatsList(request.Sorting, request.MaxResultCount, request.SkipCount, request.NameFilter);
        var dtos = ObjectMapper.Map<List<ChatRoomEntity>, List<ChatRoomDto>>(list.Value);
        result = new PagedResultDto<ChatRoomDto>(list.Key, dtos);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
