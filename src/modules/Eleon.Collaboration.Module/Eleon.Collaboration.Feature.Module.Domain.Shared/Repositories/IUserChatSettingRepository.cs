using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Collaboration.Feature.Module.Entities;

namespace VPortal.Collaboration.Feature.Module.Repositories
{
  public interface IUserChatSettingRepository : IBasicRepository<UserChatSettingEntity, Guid>
  {
    Task<UserChatSettingEntity> GetChatSettingAsync(Guid userId, Guid chatRoomId);
    Task<List<Guid>> GetMutedUsers(Guid chatRoomId);
    Task<int> UnarchiveChatAsync(Guid chatRoomId, List<Guid> userIds);
  }
}
