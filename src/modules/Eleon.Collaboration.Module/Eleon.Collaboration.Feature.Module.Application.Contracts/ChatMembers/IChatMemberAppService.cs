using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VPortal.Collaboration.Feature.Module.Chats;

namespace VPortal.Collaboration.Feature.Module.ChatMembers
{
  public interface IChatMemberAppService : IApplicationService
  {
    Task<bool> AddChatMembers(AddChatMembersRequestDto request);
    Task<bool> KickChatMembers(KickChatMembersRequestDto request);
    Task<bool> LeaveChat(Guid chatId, bool closeGroup);
    Task<bool> SetMemberRole(Guid chatId, Guid userId, ChatMemberRole memberRole);
    Task<List<ChatMemberInfo>> GetChatMembers(Guid chatId);
    Task<bool> CheckMembership(Guid chatId);
    Task<bool> JoinChat(Guid chatId);
    Task<bool> JoinChatByUserAsync(Guid chatId, Guid userId);
  }
}
