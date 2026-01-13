using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using VPortal.Collaboration.Feature.Module.ChatMessages;

namespace VPortal.Collaboration.Feature.Module.ChatInteractions
{
  public interface IChatInteractionAppService : IApplicationService
  {
    Task<ChatMessageDto> SendTextMessage(SendTextMessageRequestDto request);
    Task<ChatMessageDto> SendDocumentMessage(SendDocumentMessageRequestDto request);
    Task<PagedResultDto<UserChatInfoDto>> GetLastChats(LastChatsRequestDto request);
    Task<List<ChatMessageDto>> OpenChat(Guid chatId, int limit);
    Task<bool> AckMessageReceived(Guid messageId);
    Task<List<ChatMessageDto>> GetChatMessages(Guid chatId, int skip, int take);
    Task<string> RetreiveDocumentMessageContent(Guid messageId);

    Task<UserChatInfoDto> GetChatByIdAsync(Guid chatId);
  }
}
