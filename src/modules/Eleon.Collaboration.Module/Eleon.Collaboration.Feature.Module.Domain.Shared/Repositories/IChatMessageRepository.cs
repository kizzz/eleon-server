using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Collaboration.Feature.Module.Chats;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Specifications;

namespace VPortal.Collaboration.Feature.Module.Repositories
{
  public interface IChatMessageRepository : IBasicRepository<ChatMessageEntity, Guid>
  {
    Task<List<ChatMessageEntity>> GetLastChatMessages(Guid chatId, int skip, int limit);
    Task<KeyValuePair<long, List<UserChatInfo>>> GetLastChatsInfo(
        ChatMemberSpecification memberSpecification,
        List<ChatRoomType> chatRoomTypeFilter,
        int skip,
        int take,
        string nameFilter,
        bool isArchived = false,
        bool isChannels = false,
        List<string> tags = null);

    Task<UserChatInfo> GetChatInfoAsync(Guid id, ChatMemberSpecification memberSpecification);
  }
}
