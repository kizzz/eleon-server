using Common.Module.Extensions;
using Logging.Module;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.EntityFrameworkCore;

namespace VPortal.Collaboration.Feature.Module.Repositories
{
  public class ChatRoomRepository : EfCoreRepository<CollaborationDbContext, ChatRoomEntity, Guid>, IChatRoomRepository
  {
    private readonly IVportalLogger<ChatRoomRepository> logger;

    public ChatRoomRepository(
        IVportalLogger<ChatRoomRepository> logger,
        IDbContextProvider<CollaborationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<List<ChatRoomEntity>> GetByIds(List<Guid> ids)
    {
      List<ChatRoomEntity> result = null;
      try
      {
        var chatRooms = await GetDbSetAsync();
        result = await chatRooms.Where(x => ids.Contains(x.Id)).ToListAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<ChatRoomEntity> GetByRefId(string refId)
    {
      ChatRoomEntity result = null;
      try
      {
        var chatRooms = await GetDbSetAsync();
        result = await chatRooms.FirstOrDefaultAsync(x => x.RefId == refId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<ChatRoomEntity>> GetChatsByOwner(Guid userId, List<ChatRoomType> types)
    {
      List<ChatRoomEntity> result = null;
      try
      {
        var chatRooms = await GetDbSetAsync();
        result = await chatRooms
            .Where(x => x.CreatorId == userId && types.Contains(x.Type))
            .ToListAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<KeyValuePair<int, List<ChatRoomEntity>>> GetChatsList(string sorting, int take, int skip, string nameFilter)
    {
      KeyValuePair<int, List<ChatRoomEntity>> result = default;
      try
      {
        var chatRooms = await GetDbSetAsync();
        string namePattern = nameFilter.NonEmpty() ? $"%{nameFilter}%" : null;
        var filtered = chatRooms.WhereIf(namePattern.NonEmpty(), x => EF.Functions.Like(x.Name, namePattern));

        var chats = await filtered
            .OrderBy(sorting)
            .Take(take)
            .Skip(skip)
            .ToListAsync();

        var count = await filtered.CountAsync();

        result = KeyValuePair.Create(count, chats);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public override async Task<IQueryable<ChatRoomEntity>> WithDetailsAsync()
    {
      return (await GetQueryableAsync()).Include(x => x.ViewChatPermissions);
    }
  }
}
