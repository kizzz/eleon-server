using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Application.Contracts.ChatRooms;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace VPortal.Collaboration.Feature.Module.ChatRooms
{
  public interface IChatRoomAppService : IApplicationService
  {
    Task<ChatRoomDto> CreateChatAsync(CreateChatRequestDto request);
    Task<bool> RenameChat(Guid chatId, string newName);
    Task<PagedResultDto<ChatRoomDto>> GetChatsList(ChatListRequestDto request);
    Task<ChatRoomDto> UpdateAsync(UpdateChatRequestDto request);
    Task CloseAsync(Guid chatId);
  }
}
