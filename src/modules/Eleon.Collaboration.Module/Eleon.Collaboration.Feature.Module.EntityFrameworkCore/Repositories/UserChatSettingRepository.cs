using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.EntityFrameworkCore;

namespace VPortal.Collaboration.Feature.Module.Repositories
{
  public class UserChatSettingRepository : EfCoreRepository<CollaborationDbContext, UserChatSettingEntity, Guid>, IUserChatSettingRepository
  {
    private readonly IVportalLogger<UserChatSettingRepository> logger;

    public UserChatSettingRepository(
        IVportalLogger<UserChatSettingRepository> logger,
        IDbContextProvider<CollaborationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<UserChatSettingEntity> GetChatSettingAsync(Guid userId, Guid chatRoomId)
    {
      UserChatSettingEntity result = null;
      try
      {
        var settings = await GetDbSetAsync();
        result = await settings.FirstOrDefaultAsync(x => x.UserId == userId && x.ChatRoomId == chatRoomId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<Guid>> GetMutedUsers(Guid chatRoomId)
    {
      List<Guid> result = null;
      try
      {
        var settings = await GetDbSetAsync();
        result = await settings
            .Where(x => x.ChatRoomId == chatRoomId && x.IsChatMuted)
            .Select(x => x.UserId)
            .ToListAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<int> UnarchiveChatAsync(Guid chatRoomId, List<Guid> userIds)
    {
      try
      {
        var settings = await GetDbSetAsync();
        return await settings
            .Where(x => x.ChatRoomId == chatRoomId && !x.IsChatMuted && x.IsArchived)
            .Where(x => userIds.Contains(x.UserId))
            .ExecuteUpdateAsync(x => x.SetProperty(c => c.IsArchived, false));
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return 0;
    }
  }
}
