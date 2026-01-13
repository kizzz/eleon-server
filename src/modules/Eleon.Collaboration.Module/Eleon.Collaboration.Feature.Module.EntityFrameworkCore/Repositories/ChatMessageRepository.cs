using AutoMapper.Execution;
using Common.Module.Extensions;
using Logging.Module;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using VPortal.Collaboration.Feature.Module.Chats;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.EntityFrameworkCore;
using VPortal.Collaboration.Feature.Module.Specifications;

namespace VPortal.Collaboration.Feature.Module.Repositories
{
  public class ChatMessageRepository : EfCoreRepository<CollaborationDbContext, ChatMessageEntity, Guid>, IChatMessageRepository
  {
    private readonly IVportalLogger<ChatMessageRepository> logger;
    private readonly IdentityUserManager _identityUserManager;
    private readonly ICurrentUser _currentUser;
    private readonly IOrganizationUnitRepository _organizationUnitRepository;

    public ChatMessageRepository(
        IVportalLogger<ChatMessageRepository> logger,
        IDbContextProvider<CollaborationDbContext> dbContextProvider,
        IdentityUserManager identityUserManager,
        ICurrentUser currentUser,
        IOrganizationUnitRepository organizationUnitRepository,
        IdentityRoleManager roleManager)
        : base(dbContextProvider)
    {
      this.logger = logger;
      _identityUserManager = identityUserManager;
      _currentUser = currentUser;
      _organizationUnitRepository = organizationUnitRepository;
    }

    public async Task<List<ChatMessageEntity>> GetLastChatMessages(Guid chatId, int skip, int limit)
    {
      List<ChatMessageEntity> result = null;
      try
      {
        var chatMessages = await GetDbSetAsync();
        result = await chatMessages
            .Where(x => x.ChatRoomId == chatId)
            .OrderByDescending(x => x.CreationTime)
            .Skip(skip)
            .Take(limit).ToListAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<KeyValuePair<long, List<UserChatInfo>>> GetLastChatsInfo(
        ChatMemberSpecification memberSpecification,
        List<ChatRoomType> chatRoomTypeFilter,
        int skip,
        int take,
        string nameFilter,
        bool isArchived = false,
        bool isChannels = false,
        List<string> tags = null)
    {
      try
      {
        string nameQuery = string.IsNullOrEmpty(nameFilter) ? null : $"%{nameFilter}%";
        var db = await GetDbContextAsync();

        var currentUserId = _currentUser.GetId();

        var membersQuery = db.ChatMembers.Where(memberSpecification.ToExpression());
        var userChatSettingsQuery = db.UserChatSettings.Where(x => x.UserId == currentUserId);
        var roomsQuery = db.ChatRooms
            .Include(x => x.ViewChatPermissions)
            .WhereIf(nameQuery.NonEmpty(), x => EF.Functions.Like(x.Name, nameQuery));

        if (tags != null && tags.Count > 0)
        {
          foreach (var tag in tags)
          {
            roomsQuery = roomsQuery
                .Where(room => room.Tags.Contains(tag));
          }
        }

        var messagesQuery = db.ChatMessages
            .Join(roomsQuery,
                msgs => msgs.ChatRoomId,
                chats => chats.Id,
                (Message, Chat) => new { Message, Chat })
            .WhereIf(isChannels, x => x.Chat.IsPublic); // check is user in role or orgunit

        var withSetting = messagesQuery
            .GroupJoin(
                userChatSettingsQuery,
                x => x.Chat.Id,
                y => y.ChatRoomId,
                (mc, settings) => new { mc.Chat, mc.Message, Settings = settings }
            )
            .SelectMany(
                x => x.Settings.DefaultIfEmpty(),
                (x, setting) => new { x.Chat, x.Message, Settings = setting }
            );

        var withMember = withSetting
            .GroupJoin(membersQuery,
                mc => mc.Chat.Id,
                members => members.ChatRoomId,
                (mc, members) => new { mc.Chat, mc.Message, Member = members, mc.Settings })
            .SelectMany(
                x => x.Member.DefaultIfEmpty(),
                (mc, member) => new { mc.Chat, mc.Message, Member = member, mc.Settings }
            );

        if (isChannels)
        {
          var user = await _identityUserManager.FindByIdAsync(currentUserId.ToString());
          var units = (await _identityUserManager.GetOrganizationUnitsAsync(user)).Select(x => x.Id.ToString());
          var roles = await _identityUserManager.GetRolesAsync(user);

          withMember = withMember
              .Where(x => x.Member == null)
              .Where(x => x.Chat.ViewChatPermissions.Count == 0 || x.Chat.ViewChatPermissions
                  .Any(p =>
                      (p.EntityType == PermissionEntityType.OrgUnit && units.Contains(p.EntityRef)) ||
                      (p.EntityType == PermissionEntityType.Role && roles.Contains(p.EntityRef))));
        }

        var joined = withMember
            .WhereIf(!isChannels, x => x.Member != null)
            .WhereIf(chatRoomTypeFilter != null && chatRoomTypeFilter.Count > 0, x => chatRoomTypeFilter.Contains(x.Chat.Type))
            .WhereIf(!isChannels, x => (x.Settings == null ? false : x.Settings.IsArchived) == isArchived);

        var resultQuery = joined
            .GroupBy(x => x.Chat.Id)
            .Select(g => new
            {
              Chat = g.First().Chat,
              LastChatMessage = g.OrderByDescending(x => x.Message.CreationTime).First().Message,
              UnreadCount = g
                    .Where(
                        x => x.Member != null &&
                        !(x.Message.Sender == memberSpecification.UserId && x.Message.SenderType == ChatMessageSenderType.User)
                        && (x.Member.LastViewedByUser == null || x.Message.CreationTime > x.Member.LastViewedByUser))
                    .Count(),
              LastDate = g.Max(x => x.Message.CreationTime),
              Member = g.First().Member,
              Settings = g.First().Settings
            });

        var queryCount = await resultQuery.CountAsync();
        var queryResult = await resultQuery
            .OrderByDescending(x => x.LastDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        var allOrgUnitIds = queryResult
            .SelectMany(x => x.Chat.ViewChatPermissions
                .Where(p => p.EntityType == PermissionEntityType.OrgUnit)
                .Select(p => p.EntityRef))
            .Distinct()
            .ToList()
            .Where(x => Guid.TryParse(x, out _))
            .Select(Guid.Parse)
            .ToList();

        var allowedOrgUnits = await _organizationUnitRepository.GetListAsync(allOrgUnitIds);

        var result = queryResult.Select(x => new UserChatInfo()
        {
          Chat = x.Chat,
          LastChatMessage = x.LastChatMessage,
          UnreadCount = x.UnreadCount,
          UserRole = x.Member?.Role ?? ChatMemberRole.Regular,
          MemberRef = x.Member?.RefId,
          MemberType = x.Member?.Type ?? ChatMemberType.Undefined,
          IsChatMuted = x.Settings?.IsChatMuted ?? false,
          IsArchived = x.Settings?.IsArchived ?? false,
          AllowedOrganizationUnits = x.Chat.ViewChatPermissions
                .Where(p => p.EntityType == PermissionEntityType.OrgUnit)
                .Select(p => allowedOrgUnits.FirstOrDefault(ou => ou.Id.ToString() == p.EntityRef))
                .Where(ou => ou != null)
                .ToList(),
          AllowedRoles = x.Chat.ViewChatPermissions
                .Where(p => p.EntityType == PermissionEntityType.Role)
                .Select(p => p.EntityRef)
                .Where(r => !string.IsNullOrEmpty(r))
                .ToList(),
        }).ToList();

        return new KeyValuePair<long, List<UserChatInfo>>(queryCount, result);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }
    }


    public async Task<UserChatInfo> GetChatInfoAsync(Guid id, ChatMemberSpecification memberSpecification)
    {
      try
      {
        var db = await GetDbContextAsync();

        var currentUserId = _currentUser.GetId();

        var membersQuery = db.ChatMembers.Where(memberSpecification.ToExpression());
        var userChatSettingsQuery = db.UserChatSettings.Where(x => x.UserId == currentUserId);
        var roomsQuery = db.ChatRooms.Include(x => x.ViewChatPermissions).Where(x => x.Id == id);

        var messagesQuery = db.ChatMessages
            .Join(roomsQuery,
                msgs => msgs.ChatRoomId,
                chats => chats.Id,
                (Message, Chat) => new { Message, Chat });

        var withSetting = messagesQuery
            .GroupJoin(
                userChatSettingsQuery,
                x => x.Chat.Id,
                y => y.ChatRoomId,
                (mc, settings) => new { mc.Chat, mc.Message, Settings = settings }
            )
            .SelectMany(
                x => x.Settings.DefaultIfEmpty(),
                (x, setting) => new { x.Chat, x.Message, Settings = setting }
            );

        var withMember = withSetting
            .GroupJoin(membersQuery,
                mc => mc.Chat.Id,
                members => members.ChatRoomId,
                (mc, members) => new { mc.Chat, mc.Message, Member = members, mc.Settings })
            .SelectMany(
                x => x.Member.DefaultIfEmpty(),
                (mc, member) => new { mc.Chat, mc.Message, Member = member, mc.Settings }
            );

        var chat = await withMember
            .GroupBy(x => x.Chat.Id)
            .Select(g => new
            {
              Chat = g.First().Chat,
              LastChatMessage = g.OrderByDescending(x => x.Message.CreationTime).First().Message,
              UnreadCount = g
                    .Where(
                        x => x.Member != null &&
                        !(x.Message.Sender == memberSpecification.UserId && x.Message.SenderType == ChatMessageSenderType.User)
                        && (x.Member.LastViewedByUser == null || x.Message.CreationTime > x.Member.LastViewedByUser))
                    .Count(),
              LastDate = g.Max(x => x.Message.CreationTime),
              Member = g.First().Member,
              Settings = g.First().Settings
            })
            .FirstOrDefaultAsync() ?? throw new EntityNotFoundException();

        if (chat.Member == null && chat.Chat.ViewChatPermissions.Count > 0)
        {
          var user = await _identityUserManager.FindByIdAsync(currentUserId.ToString());
          var units = (await _identityUserManager.GetOrganizationUnitsAsync(user)).Select(x => x.Id.ToString());
          var roles = await _identityUserManager.GetRolesAsync(user);

          if (!chat.Chat.ViewChatPermissions.Any(p =>
                      (p.EntityType == PermissionEntityType.OrgUnit && units.Contains(p.EntityRef)) ||
                      (p.EntityType == PermissionEntityType.Role && roles.Contains(p.EntityRef))))
          {
            throw new EntityNotFoundException();
          }
        }

        var allOrgUnitIds = chat.Chat.ViewChatPermissions
            .Where(p => p.EntityType == PermissionEntityType.OrgUnit)
            .Select(p => p.EntityRef)
            .Distinct()
            .ToList()
            .Where(x => Guid.TryParse(x, out _))
            .Select(Guid.Parse)
            .ToList();

        var allowedOrgUnits = await _organizationUnitRepository.GetListAsync(allOrgUnitIds);

        return new UserChatInfo()
        {
          Chat = chat.Chat,
          LastChatMessage = chat.LastChatMessage,
          UnreadCount = chat.UnreadCount,
          UserRole = chat.Member?.Role ?? ChatMemberRole.Regular,
          MemberRef = chat.Member?.RefId,
          MemberType = chat.Member?.Type ?? ChatMemberType.Undefined,
          IsChatMuted = chat.Settings?.IsChatMuted ?? false,
          IsArchived = chat.Settings?.IsArchived ?? false,
          AllowedOrganizationUnits = allowedOrgUnits,
          AllowedRoles = chat.Chat.ViewChatPermissions
            .Where(p => p.EntityType == PermissionEntityType.Role)
            .Select(p => p.EntityRef)
            .Distinct()
            .Where(x => !string.IsNullOrEmpty(x))
            .ToList(),
        };
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }
    }
  }
}
