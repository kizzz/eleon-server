using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Specifications;

namespace VPortal.Collaboration.Feature.Module.Repositories
{
  public interface IChatMemberRepository : IBasicRepository<ChatMemberEntity, Guid>
  {
    Task<List<ChatMemberEntity>> GetUserMemberships(Guid userId);
    Task<ChatMemberEntity> GetByUser(Guid chatId, Guid userId);
    Task<List<ChatMemberEntity>> GetByMember(Guid chatId, ChatMemberSpecification memberSpecification);
    Task<int> GetMembershipsCount(ChatMemberSpecification memberSpecification);
    Task<List<ChatMemberEntity>> GetByRole(Guid chatId, List<ChatMemberRole> types);
    Task<List<ChatMemberEntity>> GetByChat(Guid chatId, List<ChatMemberType> typeFilter = null);
    Task<Dictionary<Guid, KeyValuePair<int, List<ChatMemberEntity>>>> GetByChats(List<Guid> chatIds);
  }
}
