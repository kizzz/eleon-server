using Logging.Module;
using Microsoft.EntityFrameworkCore;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.EntityFrameworkCore;
using VPortal.Collaboration.Feature.Module.Specifications;

namespace VPortal.Collaboration.Feature.Module.Repositories
{
  public class ChatMemberRepository : EfCoreRepository<CollaborationDbContext, ChatMemberEntity, Guid>, IChatMemberRepository
  {
    private readonly IVportalLogger<ChatMemberRepository> logger;

    public ChatMemberRepository(
        IVportalLogger<ChatMemberRepository> logger,
        IDbContextProvider<CollaborationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<List<ChatMemberEntity>> GetByChat(Guid chatId, List<ChatMemberType> typeFilter = null)
    {
      List<ChatMemberEntity> result = null;
      try
      {
        var chatMembers = await GetDbSetAsync();
        result = await chatMembers
            .Where(x => x.ChatRoomId == chatId)
            .WhereIf(!typeFilter.IsNullOrEmpty(), x => typeFilter.Contains(x.Type))
            .ToListAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<ChatMemberEntity>> GetByRole(Guid chatId, List<ChatMemberRole> roles)
    {
      List<ChatMemberEntity> result = null;
      try
      {
        var chatMembers = await GetDbSetAsync();
        result = await chatMembers.Where(x => x.ChatRoomId == chatId && roles.Contains(x.Role)).ToListAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<ChatMemberEntity> GetByUser(Guid chatId, Guid userId)
    {
      ChatMemberEntity result = null;
      try
      {
        string refId = userId.ToString();
        var chatMembers = await GetDbSetAsync();
        result = await chatMembers
            .Where(x => x.ChatRoomId == chatId && x.Type == ChatMemberType.User && x.RefId == refId)
            .FirstOrDefaultAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<ChatMemberEntity>> GetByMember(Guid chatId, ChatMemberSpecification memberSpecification)
    {
      List<ChatMemberEntity> result = null;
      try
      {
        var chatMembers = await GetDbSetAsync();
        result = await chatMembers
            .Where(x => x.ChatRoomId == chatId)
            .Where(memberSpecification.ToExpression())
            .ToListAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<int> GetMembershipsCount(ChatMemberSpecification memberSpecification)
    {
      int result = 0;
      try
      {
        var chatMembers = await GetDbSetAsync();
        result = await chatMembers.Where(memberSpecification.ToExpression()).CountAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<Dictionary<Guid, KeyValuePair<int, List<ChatMemberEntity>>>> GetByChats(List<Guid> chatIds)
    {
      Dictionary<Guid, KeyValuePair<int, List<ChatMemberEntity>>> result = null;
      try
      {

        var members = await GetDbSetAsync();
        var chatMembers = await members
            .Where(x => chatIds.Contains(x.ChatRoomId))
            .Where(x => x.Type == ChatMemberType.User)
            .GroupBy(x => x.ChatRoomId)
            .Select(x => new
            {
              chatId = x.Key,
              count = x.Count(),
              members = x.Select(g => g),
            })
            .ToListAsync();

        result = chatMembers
            .ToDictionary(
                x => x.chatId,
                x => new KeyValuePair<int, List<ChatMemberEntity>>(
                    x.count,
                    x.members.ToList()));
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<ChatMemberEntity>> GetUserMemberships(Guid userId)
    {
      List<ChatMemberEntity> result = null;
      try
      {
        var chatMembers = await GetDbSetAsync();
        string refId = userId.ToString();
        result = await chatMembers
            .Where(x => x.Type == ChatMemberType.User && x.RefId == refId)
            .ToListAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
