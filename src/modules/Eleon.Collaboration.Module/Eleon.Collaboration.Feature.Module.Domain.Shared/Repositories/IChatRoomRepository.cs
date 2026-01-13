using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Collaboration.Feature.Module.Entities;

namespace VPortal.Collaboration.Feature.Module.Repositories
{
  public interface IChatRoomRepository : IBasicRepository<ChatRoomEntity, Guid>
  {
    Task<List<ChatRoomEntity>> GetByIds(List<Guid> ids);
    Task<ChatRoomEntity> GetByRefId(string refId);
    Task<KeyValuePair<int, List<ChatRoomEntity>>> GetChatsList(string sorting, int take, int skip, string nameFilter);
    Task<List<ChatRoomEntity>> GetChatsByOwner(Guid userId, List<ChatRoomType> types);
  }
}
