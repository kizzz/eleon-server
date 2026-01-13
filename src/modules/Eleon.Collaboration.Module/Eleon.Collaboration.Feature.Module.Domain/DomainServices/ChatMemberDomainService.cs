using Common.Module.Extensions;
using EleonsoftAbp.Auth;
using Logging.Module;
using Microsoft.AspNetCore.Http;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Volo.Abp.Validation;
using VPortal.Collaboration.Feature.Module.Chats;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Repositories;
using VPortal.Collaboration.Feature.Module.Specifications;
using VPortal.Collaboration.Feature.Module.Users;

namespace VPortal.Collaboration.Feature.Module.DomainServices
{

  public class ChatMemberDomainService : DomainService
  {
    private readonly IVportalLogger<ChatMemberDomainService> logger;
    private readonly ICurrentUser currentUser;
    private readonly IdentityUserManager userManager;
    private readonly IChatRoomRepository chatRoomRepository;
    private readonly ChatUserHelperService chatUserHelper;
    private readonly ChatMessageDomainService chatMessageDomainService;
    private readonly IChatMemberRepository chatMemberRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChatMemberDomainService(
        IVportalLogger<ChatMemberDomainService> logger,
        ICurrentUser currentUser,
        IdentityUserManager userManager,
        IChatRoomRepository chatRoomRepository,
        ChatUserHelperService chatUserHelper,
        ChatMessageDomainService chatMessageDomainService,
        IChatMemberRepository chatMemberRepository,
        IHttpContextAccessor httpContextAccessor)
    {
      this.logger = logger;
      this.currentUser = currentUser;
      this.userManager = userManager;
      this.chatRoomRepository = chatRoomRepository;
      this.chatUserHelper = chatUserHelper;
      this.chatMessageDomainService = chatMessageDomainService;
      this.chatMemberRepository = chatMemberRepository;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<ChatMemberEntity>> ForceAddUserMembers(Guid chatId, Dictionary<Guid, ChatMemberRole?> users, ChatMemberRole? defaultRole = null)
    {
      var members = users
          .Select(user => new ChatMemberEntity(GuidGenerator.Create(), user.Key.ToString(), chatId, user.Value ?? defaultRole ?? ChatMemberRole.Regular))
          .ToList();
      await chatMemberRepository.InsertManyAsync(members, true);
      return members;
    }

    public async Task<List<ChatMemberEntity>> ForceAddRoleMembers(Guid chatId, List<string> roles, ChatMemberRole memberRole)
    {
      var members = roles
          .Select(role => new ChatMemberEntity(GuidGenerator.Create(), role, chatId, memberRole)
          {
            Type = ChatMemberType.Role,
          })
          .ToList();
      await chatMemberRepository.InsertManyAsync(members, true);
      return members;
    }

    public async Task<List<ChatMemberEntity>> AddMembers(Guid chatId, List<Guid> userIds)
    {
      var chat = await chatRoomRepository.GetAsync(chatId);
      if (chat.Type == ChatRoomType.Private)
      {
        throw new Exception("Unable to add a new member to the private chat.");
      }

      await EnsureCurrentUserMembership(chatId);

      if (userIds.IsNullOrEmpty())
      {
        throw new Exception("You should specify the IDs of the users to add.");
      }

      var members = userIds
          .Select(userId => new ChatMemberEntity(GuidGenerator.Create(), userId.ToString(), chatId, ChatMemberRole.Regular))
          .ToList();
      await chatMemberRepository.InsertManyAsync(members, true);

      await chatMessageDomainService.AddMembersAddedMessage(chatId, (Guid)currentUser.Id, userIds);

      return members;
    }

    public async Task SetOwners(Guid chatId, List<Guid> ownerRefs)
    {
      var existingOwners = await chatMemberRepository.GetByRole(chatId, ChatMemberRole.Owner.ToSingleItemList());
      foreach (var existingOwner in existingOwners)
      {
        existingOwner.Role = ChatMemberRole.Administrator;
      }

      await chatMemberRepository.UpdateManyAsync(existingOwners, true);

      foreach (var ownerRef in ownerRefs)
      {
        var newOwner = await chatMemberRepository.GetByUser(chatId, ownerRef);
        newOwner.Role = ChatMemberRole.Owner;
        await chatMemberRepository.UpdateAsync(newOwner, true);
      }
    }

    public async Task LeaveChat(Guid chatId, bool closeGroup)
    {
      var currentMember = await EnsureCurrentUserMembership(chatId);

      if (currentMember.Role == ChatMemberRole.Owner && closeGroup)
      {
        var members = await chatMemberRepository.GetByChat(chatId, new List<ChatMemberType> { ChatMemberType.User });
        await chatMemberRepository.DeleteManyAsync(members, true);
      }
      else
      {
        await chatMemberRepository.DeleteAsync(currentMember, true);
      }

      if (currentMember.Role == ChatMemberRole.Owner && !closeGroup)
      {
        //var possibleOwners = await chatMemberRepository.GetByChat(chatId, new List<ChatMemberType> { ChatMemberType.User });
        //var nextOwner = possibleOwners.OrderByDescending(x => x.Role).FirstOrDefault();
        //if (nextOwner != null && nextOwner.Role != ChatMemberRole.Owner)
        //{
        //    await SetOwners(chatId, Guid.Parse(nextOwner.RefId).ToSingleItemList());
        //}
      }

      await chatMessageDomainService.AddUserLeftMessage(chatId, currentUser.Id.Value);
    }

    public async Task KickMembers(Guid chatId, List<Guid> usersToKick)
    {
      var currentMember = await EnsureCurrentUserAdminRights(chatId);

      foreach (var userToKick in usersToKick)
      {
        var memberToKick = await chatMemberRepository.GetByUser(chatId, userToKick);
        if (currentMember.Role != ChatMemberRole.Owner && memberToKick.Role is ChatMemberRole.Owner or ChatMemberRole.Administrator)
        {
          throw new Exception("The user does not have the right to kick this user.");
        }

        await chatMemberRepository.DeleteAsync(memberToKick, true);
      }

      await chatMessageDomainService.AddMembersKickedMessage(chatId, (Guid)currentUser.Id, usersToKick);
    }

    public async Task SetMemberRole(Guid chatId, Guid userId, ChatMemberRole role)
    {
      var currentMember = await EnsureCurrentUserAdminRights(chatId);

      if (currentMember.Role != ChatMemberRole.Owner && role is ChatMemberRole.Owner or ChatMemberRole.Administrator)
      {
        throw new Exception("Only owner can set administrators.");
      }

      if (role == ChatMemberRole.Owner)
      {
        await SetOwners(chatId, userId.ToSingleItemList());
      }
      else
      {
        var memberToChange = await chatMemberRepository.GetByUser(chatId, userId);
        memberToChange.Role = role;
        await chatMemberRepository.UpdateAsync(memberToChange, true);
      }
    }

    public async Task<bool> CheckMembership(Guid chatId)
    {
      var memberSpec = await GetCurrentMemberSpec();
      var memberships = await chatMemberRepository.GetByMember(chatId, memberSpec);
      return !memberships.IsNullOrEmpty();
    }

    public async Task JoinChatAsync(Guid chatId, Guid userId)
    {
      var chat = await chatRoomRepository.GetAsync(chatId, true);

      var existing = await chatMemberRepository.GetByUser(chatId, userId);
      if (existing != null)
      {
        return;
      }

      // skip permission validation if the request is made by an API key
      if (string.IsNullOrEmpty(_httpContextAccessor.HttpContext.User.GetApiKeyId()))
      {
        await EnsureHasAccessToPublicChat(chatId);
      }

      var member = new ChatMemberEntity(GuidGenerator.Create(), userId.ToString(), chatId, chat.DefaultRole);
      await chatMemberRepository.InsertAsync(member, true);

      await chatMessageDomainService.AddMemberJoinedMessage(chatId, userId);
    }

    public async Task<List<ChatMemberInfo>> GetChatMembers(Guid chatId)
    {
      var currentMember = await EnsureCurrentUserMembership(chatId);
      var members = await chatMemberRepository.GetByChat(chatId, new List<ChatMemberType> { ChatMemberType.User });
      var infos = new List<ChatMemberInfo>();
      foreach (var member in members.Where(x => x.Type == ChatMemberType.User))
      {
        var user = Guid.Parse(member.RefId);
        var info = await chatUserHelper.GetUserInfo(user);
        infos.Add(info);
      }

      return infos;
    }

    public async Task ForceRemoveUserMemberships(Guid userId)
    {
      var members = await chatMemberRepository.GetUserMemberships(userId);
      await chatMemberRepository.DeleteManyAsync(members, true);
    }

    public async Task ForceRemoveChatMembers(Guid chatId)
    {
      var members = await chatMemberRepository.GetByChat(chatId);
      await chatMemberRepository.DeleteManyAsync(members, true);
    }

    internal async Task<ChatMemberEntity> EnsureCurrentUserOwner(Guid chatId)
    {
      var member = await EnsureCurrentUserMembership(chatId);
      if (member.Role is not ChatMemberRole.Owner)
      {
        throw new Exception("The user does not have the proper rights to perform the action.");
      }

      return member;
    }

    internal async Task<ChatMemberEntity> EnsureCurrentUserAdminRights(Guid chatId)
    {
      var member = await EnsureCurrentUserMembership(chatId);
      if (member.Role is not ChatMemberRole.Owner and not ChatMemberRole.Administrator)
      {
        throw new Exception("The user does not have the proper rights to perform the action.");
      }

      return member;
    }

    internal async Task<ChatMemberEntity> EnsureCurrentUserMembership(Guid chatId)
    {
      var memberSpec = await GetCurrentMemberSpec();
      var memberships = await chatMemberRepository.GetByMember(chatId, memberSpec);
      if (memberships.IsNullOrEmpty())
      {
        throw new Exception("Current user is not a member of the chat.");
      }

      return memberships.FirstOrDefault(x => x.Type == ChatMemberType.User) ?? memberships.First();
    }

    internal async Task<ChatMemberEntity> EnsureWritePermission(Guid chatId)
    {
      var member = await EnsureCurrentUserMembership(chatId);
      if (member.Role is not ChatMemberRole.Owner and not ChatMemberRole.Administrator and not ChatMemberRole.Regular)
      {
        throw new Exception("The user does not have the proper rights to perform the action.");
      }

      return member;
    }

    internal async Task<ChatMemberEntity> EnsureReadPermission(Guid chatId)
    {
      var memberSpec = await GetCurrentMemberSpec();
      var memberships = await chatMemberRepository.GetByMember(chatId, memberSpec);
      if (memberships.IsNullOrEmpty())
      {
        await EnsureHasAccessToPublicChat(chatId);
        return null;
      }

      return memberships.FirstOrDefault(x => x.Type == ChatMemberType.User) ?? memberships.First();
    }

    internal async Task EnsureHasAccessToPublicChat(Guid chatId)
    {
      var chat = await chatRoomRepository.GetAsync(chatId, true);

      if (!chat.IsPublic)
      {
        throw new UserFriendlyException("View chat not allowed");
      }

      if ((chat?.ViewChatPermissions.Count ?? 0) > 0)
      {
        var user = await userManager.FindByIdAsync(currentUser.Id.Value.ToString());

        bool canViewPublicChat = false;
        foreach (var permission in chat.ViewChatPermissions)
        {
          if (permission.EntityType == PermissionEntityType.Role)
          {
            if (currentUser.IsInRole(permission.EntityRef))
            {
              canViewPublicChat = true;
              break;
            }
          }

          if (permission.EntityType == PermissionEntityType.OrgUnit && Guid.TryParse(permission.EntityRef, out var orgUnitId))
          {
            if (user.IsInOrganizationUnit(orgUnitId))
            {
              canViewPublicChat = true;
              break;
            }
          }
        }

        if (canViewPublicChat == false)
        {
          throw new UserFriendlyException("View chat not allowed");
        }
      }
    }

    internal async Task<ChatMemberSpecification> GetCurrentMemberSpec()
    {
      var user = await userManager.GetByIdAsync(currentUser.Id.Value);
      var roles = await userManager.GetRolesAsync(user);
      return new ChatMemberSpecification(user.Id, roles.ToList());
    }
  }
}
